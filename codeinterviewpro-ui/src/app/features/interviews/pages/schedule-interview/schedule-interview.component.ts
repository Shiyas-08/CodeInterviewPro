import { toast } from 'src/app/core/services/toast';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-schedule-interview',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './schedule-interview.component.html'
})
export class ScheduleInterviewComponent {

  interviewId = '';

  form = {
    startTime: '',
    endTime: ''
  };

  loading = false;

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
    toast.warning('End time must be after start time');
    return;
  }

  const payload = {
    startTime: start.toISOString(),
    endTime: end.toISOString()
  };

  this.loading = true;

  this.service.scheduleInterview(
    this.interviewId,
    payload
  ).subscribe({

    next: (res) => {
      this.loading = false;
      toast.success(res?.message || 'Interview Scheduled');

      this.router.navigate([
        '/dashboard/interviews',
        this.interviewId,
        'invite'
      ]);
    },

    error: (err) => {
      this.loading = false;
      toast.error(err?.error?.message || 'Schedule failed');
    }

  });
}
}