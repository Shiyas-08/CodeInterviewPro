import { toast } from 'src/app/core/services/toast';
// interview-room.component.ts

import { Component, OnInit, OnDestroy, HostListener, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { WebrtcService } from 'src/app/core/services/webrtc.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-interview-room',
  templateUrl: './interview-room.component.html'
})
export class InterviewRoomComponent implements OnInit, OnDestroy {

  token = '';
  loading = true;
  isFullscreen = false;
  isTimeUp = false;
  isSubmitting = false;
  isStopped = false;

  @ViewChild('localVideo') localVideo!: ElementRef<HTMLVideoElement>;
  @ViewChild('remoteVideo') remoteVideo!: ElementRef<HTMLVideoElement>;

  realSessionId = '';
  subs: Subscription[] = [];

  session: any = {
    title: 'Coding Interview',
    questions: []
  };

  selectedQuestion: any = null;

  code = '';
  output = '';

  timer = 0;
  interval: any;
  autoSaveInterval: any;

  isRunning = false;
  isVideoFullscreen = false;

  editorOptions: any = {
    theme: 'vs-dark',
    language: 'csharp',
    automaticLayout: true,
    fontSize: 16,
    lineNumbers: 'on',
    roundedSelection: true,
    scrollBeyondLastLine: false,
    readOnly: false,
    minimap: { enabled: false },
    padding: { top: 16, bottom: 16 }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: InterviewService,
    private signalrService: SignalrService,
    public webrtcService: WebrtcService
  ) { }

  ngOnInit(): void {

    this.token = this.route.snapshot.paramMap.get('id') || '';

    if (!this.token) {
      toast.error('Invalid interview token');
      this.loading = false;
      return;
    }

    this.startInterview();
  }

  ngAfterViewInit(): void {
    if (this.token) {
      this.setupWebRTC();
    }
  }

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any): void {
    const sid = this.realSessionId || this.token;
    if (sid) {
      this.signalrService.sendCandidateStatusChanged(sid, false);
    }
  }

  ngOnDestroy(): void {
    const sid = this.realSessionId || this.token;
    if (sid) {
      this.signalrService.sendCandidateStatusChanged(sid, false);
    }
    if (this.interval) clearInterval(this.interval);
    if (this.autoSaveInterval) clearInterval(this.autoSaveInterval);
    this.webrtcService.endCall();
    this.subs.forEach(s => s.unsubscribe());
  }

  // ===================================================
  // WEBRTC VIDEO CALL
  // ===================================================
  async setupWebRTC() {
    try {
      const stream = await this.webrtcService.initLocalStream();
      if (this.localVideo && this.localVideo.nativeElement) {
        this.localVideo.nativeElement.srcObject = stream;
      }
    } catch (e) {
      console.error('Candidate failed to load camera', e);
    }

    this.webrtcService.createPeerConnection((candidate) => {
      // Candidate sends ICE to HR
      const sid = this.realSessionId || this.token;
      this.signalrService.sendIceCandidate(sid, JSON.stringify(candidate), false);
    });

    this.subs.push(
      this.webrtcService.remoteStream$.subscribe(stream => {
        if (this.remoteVideo && this.remoteVideo.nativeElement) {
          this.remoteVideo.nativeElement.srcObject = stream;
        }
      }),
      
      // Candidate receives Offer from HR
      this.signalrService.webRTCOffer$.subscribe(async (offerStr) => {
        const sid = this.realSessionId || this.token;
        const offer = JSON.parse(offerStr);
        const answer = await this.webrtcService.handleOffer(offer);
        // Candidate sends Answer back to HR
        this.signalrService.sendWebRTCAnswer(sid, JSON.stringify(answer), false);
      }),

      // Candidate receives ICE from HR
      this.signalrService.iceCandidate$.subscribe(async (candidateStr) => {
        const candidate = JSON.parse(candidateStr);
        await this.webrtcService.handleIceCandidate(candidate);
      })
    );
  }

  // ===================================================
  // START FIRST TIME / RESUME IF ALREADY STARTED
  // ===================================================
  startInterview() {

    this.service.startInterview(this.token).subscribe({
      next: (res: any) => {
        this.bindData(res);
      },

      error: (err) => {

        const msg = err?.error?.message || '';

        if (msg.includes('already started')) {
          this.resumeInterview();
          return;
        }

        toast.error('Unable to start interview');
        this.loading = false;
      }
    });
  }

  // ===================================================
  // NEW RESUME API
  // ===================================================
  resumeInterview() {

    this.service.resumeInterview(this.token).subscribe({
      next: (res: any) => {
        this.bindData(res);
      },

      error: () => {
        toast.error('Unable to resume interview');
        this.loading = false;
      }
    });
  }

  // ===================================================
  // COMMON DATA BINDING
  // ===================================================
  bindData(res: any) {

    this.session = res.data || res || {};

    if (!this.session.title) {
      this.session.title = 'Coding Interview';
    }

    if (!this.session.questions) {
      this.session.questions = [];
    }

    // normalize id
    this.session.questions = this.session.questions.map((q: any) => ({
      ...q,
      id: q.id || q.questionId
    }));

    if (this.session.questions.length > 0) {
      this.selectQuestion(this.session.questions[0]);
    }

    this.timer =
      this.session.remainingSeconds ||
      (this.session.durationMinutes * 60) ||
      0;

    this.loading = false;

    // Start SignalR
    // Check all possible casing for SessionId from the backend DTO
    this.realSessionId = this.session.id || this.session.Id || this.session.sessionId || this.session.SessionId || this.token;
    
    this.signalrService.startConnection().then(() => {
      this.signalrService.joinCandidateRoom(this.realSessionId);
      this.signalrService.sendCandidateStatusChanged(this.realSessionId, true);
    });

    this.subs.push(
      this.signalrService.interviewStopped$.subscribe(() => {
        this.handleInterviewStopped();
      }),
      this.signalrService.pingCandidate$.subscribe(() => {
        this.signalrService.sendCandidateStatusChanged(this.realSessionId, true);
      })
    );

    this.startTimer();
    this.startAutoSave();

    // If session is already completed, lock it down
    if (this.session.status === 2 || this.session.status === 'Completed') {
      this.handleInterviewStopped();
    }
  }

  // ===================================================
  // QUESTION SELECT
  // ===================================================
  selectQuestion(q: any) {

    this.selectedQuestion = q;

    const savedCode = localStorage.getItem(
      'interview_' + this.token + '_' + q.id
    );

    const rawCode = savedCode || q.starterCode || '';

    this.code = rawCode.replace(/\\n/g, '\n');

    this.editorOptions = {
      ...this.editorOptions,
      language: this.mapLanguage(q.language)
    };
  }

  mapLanguage(language: any): string {

    if (language === null || language === undefined)
      return 'csharp';

    const lang = String(language).toLowerCase();

    switch (lang) {
      case '0':
      case 'csharp':
        return 'csharp';

      case '1':
      case 'python':
        return 'python';

      case '2':
      case 'javascript':
        return 'javascript';

      case '3':
      case 'java':
        return 'java';

      case '4':
      case 'go':
        return 'go';

      default:
        return 'csharp';
    }
  }

  // ===================================================
  // TIMER
  // ===================================================
  startTimer() {

    if (this.interval) clearInterval(this.interval);

    this.interval = setInterval(() => {

      if (this.timer > 0) {
        this.timer--;
        if (this.timer % 5 === 0) { // Sync timer every 5s
            this.signalrService.sendTimerUpdated(this.realSessionId, this.timer);
        }
      } else {
        clearInterval(this.interval);
        this.handleTimeUp();
      }

    }, 1000);
  }

  handleTimeUp() {

    this.isTimeUp = true;

    this.editorOptions = {
      ...this.editorOptions,
      readOnly: true
    };

    toast.warning('Time is up! Auto submitting...');

    this.submit();
  }

  handleInterviewStopped() {
    this.isStopped = true;
    this.isTimeUp = true;
    if (this.interval) clearInterval(this.interval);
    this.editorOptions = { ...this.editorOptions, readOnly: true };
    
    // Show a high-priority toast
    toast.warning('This interview has been stopped by the HR. Your session is now locked.', { duration: 10000 });
  }

  formatTime(seconds: number): string {

    const m = Math.floor(seconds / 60);
    const s = seconds % 60;

    return `${m}:${s < 10 ? '0' : ''}${s}`;
  }

  toggleFullscreen() {
    this.isFullscreen = !this.isFullscreen;
  }

  // ===================================================
  // AUTO SAVE
  // ===================================================
  startAutoSave() {

    this.autoSaveInterval = setInterval(() => {
      console.log(`[AutoSave] Heartbeat for ${this.realSessionId}`);
      if (this.selectedQuestion?.id && !this.isTimeUp) {

        localStorage.setItem(
          'interview_' + this.realSessionId + '_' + this.selectedQuestion.id,
          this.code
        );
        // Broadcast code to HR
        this.signalrService.sendCodeChanged(this.realSessionId, this.code);
      }

    }, 5000);
  }

  // ===================================================
  // RUN CODE
  // ===================================================
  runCode() {

    if (!this.selectedQuestion || this.isTimeUp || this.isStopped)
      return;

    const payload = {
      token: this.token,
      questionId: this.selectedQuestion.id,
      code: this.code,
      language: this.selectedQuestion.language || 0
    };

    this.isRunning = true;

    this.service.runCode(payload).subscribe({
      next: (res: any) => {

        this.output =
`Passed: ${res.passed ?? 0}
Failed: ${res.failed ?? 0}
Score: ${res.finalScore ?? 0}

AI FEEDBACK:
${res.aiFeedback || 'No feedback available.'}`;

        this.isRunning = false;
      },

      error: () => {
        this.output = 'Execution Failed.';
        this.isRunning = false;
      }
    });
  }

  // ===================================================
  // SUBMIT
  // ===================================================
  submit() {

    if (!this.selectedQuestion || this.isStopped)
      return;

    this.isSubmitting = true;

    const payload = {
      token: this.token,
      questionId: this.selectedQuestion.id,
      code: this.code,
      language: this.selectedQuestion.language || 0
    };

    this.service.submitInterview(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        // Tell HR we submitted
        this.signalrService.sendInterviewSubmitted(this.realSessionId);

        if (!this.isTimeUp) {
          toast.success('Submission successful!');
        }
      },

      error: () => {
        this.isSubmitting = false;
        toast.error('Submission failed');
      }
    });
  }
}