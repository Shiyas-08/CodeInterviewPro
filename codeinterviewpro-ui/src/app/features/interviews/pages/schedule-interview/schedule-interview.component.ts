import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-schedule-interview',
  templateUrl: './schedule-interview.component.html'
})
export class ScheduleInterviewComponent {

  interviewId = '';

  form = {
    startTime: '',
    endTime: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: InterviewService
  ) {
    this.interviewId =
      this.route.snapshot.paramMap.get('id') || '';
  }

  submit() {

  const start = new Date(this.form.startTime);
  const end = new Date(this.form.endTime);

  // Frontend validation
  if (start >= end) {
    alert('End time must be after start time');
    return;
  }

  const payload = {
    startTime: start.toISOString(),
    endTime: end.toISOString()
  };

  this.service.scheduleInterview(
    this.interviewId,
    payload
  ).subscribe({

    next: (res) => {

      alert(res?.message || 'Interview Scheduled');

      this.router.navigate([
        '/dashboard/interviews',
        this.interviewId,
        'invite'
      ]);
    },

    error: (err) => {
      alert(err?.error?.message || 'Schedule failed');
    }

  });
}
}