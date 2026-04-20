import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-interview-room',
  templateUrl: './interview-room.component.html'
})
export class InterviewRoomComponent implements OnInit, OnDestroy {

  token = '';
  loading = true;

  session: any = {
    questions: []
  };

  selectedQuestion: any = null;

  code = '';
  output = '';

  timer = 0;
  interval: any;
  autoSaveInterval: any;

  isRunning = false;

  editorOptions: any = {
    theme: 'vs-dark',
    language: 'csharp',
    automaticLayout: true,
    fontSize: 14,
    minimap: { enabled: false }
  };

  constructor(
    private route: ActivatedRoute,
    private service: InterviewService
  ) { }

  ngOnInit(): void {

    this.token =
      this.route.snapshot.paramMap.get('id') || '';

    this.startInterview();
  }

  ngOnDestroy(): void {

    if (this.interval) {
      clearInterval(this.interval);
    }

    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
    }
  }

  startInterview() {

    this.service.startInterview(this.token).subscribe({
      next: (res: any) => {
        this.bindData(res);
      },
      error: (err) => {

        if (err.error?.message === 'Interview already started') {
          this.loadExistingSession();
        } else {
          alert('Unable to start interview');
          this.loading = false;
        }
      }
    });
  }

  loadExistingSession() {

    this.service.getSession(this.token).subscribe({
      next: (res: any) => {
        this.bindData(res);
      },
      error: () => {
        alert('Unable to load session');
        this.loading = false;
      }
    });
  }

  bindData(res: any) {

    this.session = res.data || res;

    if (!this.session.questions) {
      this.session.questions = [];
    }

    if (this.session.questions.length > 0) {
      this.selectQuestion(this.session.questions[0]);
    }

    this.timer =
      this.session.remainingSeconds ||
      (this.session.durationMinutes * 60) ||
      0;

    this.loading = false;

    this.startTimer();

    this.startAutoSave();

    console.log('SESSION DATA:', this.session);
  }

  selectQuestion(q: any) {

    console.log('FULL QUESTION OBJECT:', q);

    q.id =
      q.id ||
      q.questionId ||
      q.interviewQuestionId;

    this.selectedQuestion = q;

    const savedCode =
      localStorage.getItem(
        'interview_' + this.token + '_' + q.id
      );

    this.code =
      savedCode ||
      q.starterCode ||
      '';

    this.editorOptions = {
      ...this.editorOptions,
      language: this.mapLanguage(q.language)
    };
  }

  mapLanguage(language: any): string {

    const lang =
      String(language).toLowerCase();

    switch (lang) {

      case 'python':
      case '1':
        return 'python';

      case 'javascript':
      case '2':
        return 'javascript';

      case 'java':
      case '3':
        return 'java';

      case 'go':
      case '4':
        return 'go';

      default:
        return 'csharp';
    }
  }

  startTimer() {

    this.interval = setInterval(() => {

      if (this.timer > 0) {
        this.timer--;
      } else {
        clearInterval(this.interval);
      }

    }, 1000);
  }

  startAutoSave() {

    this.autoSaveInterval = setInterval(() => {

      if (this.selectedQuestion?.id) {

        localStorage.setItem(
          'interview_' +
          this.token +
          '_' +
          this.selectedQuestion.id,
          this.code
        );
      }

    }, 5000);
  }

  runCode() {

    if (!this.selectedQuestion) {
      alert('Select question first');
      return;
    }

    const payload = {
      token: this.token,
      questionId: this.selectedQuestion.id,
      code: this.code,
      language:
        this.selectedQuestion.language || 0
    };

    console.log('RUN PAYLOAD:', payload);

    this.isRunning = true;

    this.service.runCode(payload).subscribe({
      next: (res: any) => {

        this.output =
          'Passed: ' + (res.passed ?? 0) +
          '\nFailed: ' + (res.failed ?? 0) +
          '\nScore: ' + (res.finalScore ?? 0) +
          '\n\n' +
          (res.aiFeedback || '');

        this.isRunning = false;
      },
      error: (err) => {

        console.log(err);

        this.output = 'Execution Failed';

        this.isRunning = false;
      }
    });
  }

  submit() {

    if (!this.selectedQuestion) {
      alert('Select question first');
      return;
    }

    const payload = {
      token: this.token,
      questionId: this.selectedQuestion.id,
      code: this.code,
      language:
        this.selectedQuestion.language || 0
    };

    console.log('SUBMIT PAYLOAD:', payload);

    this.service.submitInterview(payload).subscribe({
      next: (res: any) => {

        console.log(res);

        alert('Submitted Successfully');
      },
      error: (err) => {

        console.log(err);

        alert('Submit failed');
      }
    });
  }
}
// import { Component, OnInit } from '@angular/core';
// import { ActivatedRoute } from '@angular/router';
// import { InterviewService } from 'src/app/core/services/interview.service';

// @Component({
//   selector: 'app-interview-room',
//   templateUrl: './interview-room.component.html'
// })
// export class InterviewRoomComponent implements OnInit {

//   token = '';
//   loading = true;
//   editorOptions = {
//   theme: 'vs-dark',
//   language: 'csharp',
//   automaticLayout: true,
//   fontSize: 14,
//   minimap: { enabled: false }
// };
//   session: any = {
//     questions: []
    
//   };

//   code = '';
//   timer = 0;
//   interval: any;

//   constructor(
//     private route: ActivatedRoute,
//     private service: InterviewService
//   ) {}

//   ngOnInit(): void {
//     this.token =
//       this.route.snapshot.paramMap.get('id') || '';

//     this.startInterview();
//   }

//   startInterview() {
//     this.service.startInterview(this.token).subscribe({
//       next: (res: any) => {
//         this.bindData(res);
//       },
//       error: (err) => {

//         if (err.error?.message === 'Interview already started') {
//           this.loadExistingSession();
//         } else {
//           alert('Unable to start interview');
//           this.loading = false;
//         }

//       }
//     });
//   }

//   loadExistingSession() {
//     this.service.getSession(this.token).subscribe({
//       next: (res: any) => {
//         this.bindData(res);
//       },
//       error: () => {
//         alert('Unable to load session');
//         this.loading = false;
//       }
//     });
//   }

//   bindData(res: any) {

//     this.session = res.data || res;

//     if (!this.session.questions) {
//       this.session.questions = [];
//     }

//     this.code = this.session.starterCode || '';

//     this.timer =
//       this.session.remainingSeconds ||
//       (this.session.durationMinutes * 60) ||
//       0;

//     this.loading = false;

//     this.startTimer();

//     console.log(this.session);
//   }

//   startTimer() {

//     if (this.interval) {
//       clearInterval(this.interval);
//     }

//     this.interval = setInterval(() => {

//       if (this.timer > 0) {
//         this.timer--;
//       } else {
//         clearInterval(this.interval);
//       }

//     }, 1000);
//   }

//   submit() {

//     const payload = {
//       interviewId: this.session.interviewId,
//       code: this.code
//     };

//     this.service.submitInterview(payload)
//       .subscribe({
//         next: () => {
//           alert('Submitted Successfully');
//         },
//         error: () => {
//           alert('Submit failed');
//         }
//       });
//   }

// }