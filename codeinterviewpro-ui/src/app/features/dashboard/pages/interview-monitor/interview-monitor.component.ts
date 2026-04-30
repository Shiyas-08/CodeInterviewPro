import { toast } from 'src/app/core/services/toast';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { WebrtcService } from 'src/app/core/services/webrtc.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';

@Component({
  selector: 'app-interview-monitor',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MonacoEditorModule],
  templateUrl: './interview-monitor.component.html',
  styleUrls: ['./interview-monitor.component.css']
})
export class InterviewMonitorComponent implements OnInit, OnDestroy, AfterViewInit {
  sessionId = '';
  sessionDetails: any = null;
  code = '';
  timer = 0;
  isOnline = false;
  subs: Subscription[] = [];
  candidateResult: any = null;
  showStopModal = false;
  stopping = false;
  isVideoFullscreen = false;
  callActive = false;
  isAudioMuted = false;
  isVideoMuted = false;

  @ViewChild('localVideo') localVideo!: ElementRef<HTMLVideoElement>;
  @ViewChild('remoteVideo') remoteVideo!: ElementRef<HTMLVideoElement>;

  editorOptions: any = {
    theme: 'vs-dark',
    language: 'csharp',
    automaticLayout: true,
    fontSize: 16,
    lineNumbers: 'on',
    readOnly: true,
    minimap: { enabled: false }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private interviewService: InterviewService,
    private signalrService: SignalrService,
    public webrtcService: WebrtcService
  ) {}

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('id') || '';

    if (this.sessionId) {
      this.loadSessionDetails();
      
      this.signalrService.startConnection().then(() => {
        // We will join the room inside loadSessionDetails once we have the REAL Session ID
        console.log('[SignalR] Connection started, waiting for session details to join room...');
      });

      this.subs.push(
        this.signalrService.codeChanged$.subscribe(data => {
          const currentId = this.sessionDetails?.id || this.sessionDetails?.Id || this.sessionId;
          if (data.sessionId?.toLowerCase() === currentId?.toLowerCase()) {
            this.code = data.code;
            this.isOnline = true; // Mark as online if we are receiving live code
          }
        }),
        this.signalrService.candidateStatusChanged$.subscribe(isOnline => {
          this.isOnline = isOnline;
          if (!isOnline) {
            this.callActive = false;
          }
        }),
        this.signalrService.timerUpdated$.subscribe(remaining => {
          this.timer = remaining;
        }),
        this.signalrService.interviewSubmitted$.subscribe(() => {
          toast.info('🔔 The candidate has submitted their interview code!');
          this.fetchCandidateResult();
        }),
        
        // Move WebRTC SignalR subscriptions here to ensure they are registered once
        this.signalrService.webRTCAnswer$.subscribe(async (answerStr) => {
          const answer = JSON.parse(answerStr);
          await this.webrtcService.handleAnswer(answer);
        }),

        this.signalrService.iceCandidate$.subscribe(async (candidateStr) => {
          const candidate = JSON.parse(candidateStr);
          await this.webrtcService.handleIceCandidate(candidate);
        })
      );
    }
  }

  ngAfterViewInit(): void {
    if (this.sessionId) {
      this.setupWebRTC();
    }
  }

  async setupWebRTC() {
    try {
      const stream = await this.webrtcService.initLocalStream();
      if (this.localVideo && this.localVideo.nativeElement) {
        this.localVideo.nativeElement.srcObject = stream;
      }
      // Sync initial state
      this.isAudioMuted = this.webrtcService.isAudioMuted;
      this.isVideoMuted = this.webrtcService.isVideoMuted;
    } catch (e) {
      console.error('HR failed to load camera', e);
      toast.error('Unable to access camera. Please check permissions.');
    }

    this.webrtcService.createPeerConnection((candidate) => {
      this.signalrService.sendIceCandidate(this.sessionId, JSON.stringify(candidate), true);
    });

    this.subs.push(
      this.webrtcService.remoteStream$.subscribe(stream => {
        if (this.remoteVideo && this.remoteVideo.nativeElement) {
          this.remoteVideo.nativeElement.srcObject = stream;
          this.callActive = true;
          toast.success("Connection established with candidate");
        }
      })
    );
  }

  toggleAudio() {
    this.webrtcService.toggleAudio();
    this.isAudioMuted = this.webrtcService.isAudioMuted;
  }

  toggleVideo() {
    this.webrtcService.toggleVideo();
    this.isVideoMuted = this.webrtcService.isVideoMuted;
  }

  async callCandidate() {
    if (!this.isOnline) {
      toast.warning("Candidate is not online yet.");
      return;
    }
    
    try {
      const offer = await this.webrtcService.createOffer();
      this.signalrService.sendWebRTCOffer(this.sessionId, JSON.stringify(offer), true);
      toast.info("Establishing secure link...");
    } catch (e) {
      console.error('Failed to create offer', e);
      toast.error('Connection failed. Retrying...');
      this.setupWebRTC();
    }
  }

  loadSessionDetails() {
    this.interviewService.getSession(this.sessionId).subscribe({
      next: (res: any) => {
        this.sessionDetails = res.data || res || {};
        
        // Handle PascalCase fallback from backend DTOs if necessary
        if (this.sessionDetails.Title && !this.sessionDetails.title) this.sessionDetails.title = this.sessionDetails.Title;
        if (this.sessionDetails.CandidateEmail && !this.sessionDetails.candidateEmail) this.sessionDetails.candidateEmail = this.sessionDetails.CandidateEmail;
        if (this.sessionDetails.DurationMinutes && !this.sessionDetails.durationMinutes) this.sessionDetails.durationMinutes = this.sessionDetails.DurationMinutes;

        // NOW JOIN THE SIGNALR ROOM WITH THE REAL ID
        const realId = this.sessionDetails.id || this.sessionDetails.Id;
        if (realId) {
          console.log(`[SignalR] Joining HR Room with Real ID: ${realId}`);
          this.signalrService.joinHRRoom(realId);
          this.signalrService.sendPingCandidate(realId);
        } else {
          // Fallback to URL ID if for some reason we don't have one in the body
          this.signalrService.joinHRRoom(this.sessionId);
          this.signalrService.sendPingCandidate(this.sessionId);
        }

        // If session is already completed, fetch results immediately
        if (this.sessionDetails.status === 4 || this.sessionDetails.status === 'Completed' || this.sessionDetails.Status === 4) {
          this.fetchCandidateResult();
        }
      },
      error: () => console.log('Failed to load session details.')
    });
  }

  fetchCandidateResult() {
    if (!this.sessionDetails?.candidateId) return;
    this.interviewService.getCandidateResult(this.sessionDetails.candidateId).subscribe({
      next: (res: any) => {
        const data = res.data || res;
        this.candidateResult = data;
        
        // Ensure totalScore is mapped correctly (it might be finalScore in some DTOs)
        if (this.candidateResult && this.candidateResult.totalScore === undefined) {
          this.candidateResult.totalScore = this.candidateResult.finalScore || 0;
        }
      },
      error: (err) => console.log('Failed to fetch candidate results', err)
    });
  }

  confirmStopInterview() {
    if (!this.sessionId) return;
    
    this.stopping = true;
    this.interviewService.stopSession(this.sessionId).subscribe({
      next: () => {
        this.stopping = false;
        this.showStopModal = false;
        toast.success('Interview stopped successfully');
        
        if (this.sessionDetails?.candidateId && this.sessionDetails?.interviewId) {
          this.router.navigate(['/admin/candidate-result', this.sessionDetails.candidateId, this.sessionDetails.interviewId]);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        this.stopping = false;
        console.error('Failed to stop interview', err);
        toast.error('Error stopping interview');
      }
    });
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s < 10 ? '0' : ''}${s}`;
  }

  ngOnDestroy(): void {
    this.webrtcService.endCall();
    this.subs.forEach(s => s.unsubscribe());
  }
}
