import { toast } from 'src/app/core/services/toast';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-create-interview',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './create-interview.component.html'
})
export class CreateInterviewComponent {

  form = {
    title: '',
    description: '',
    durationMinutes: 60
  };

  loading = false;

  constructor(
    private interviewService: InterviewService,
    private router: Router
  ) {}

  submit() {

    this.loading = true;

    this.interviewService.createInterview(this.form)
      .subscribe({

        next: (res) => {

          const interviewId = res.data;

          this.loading = false;

          this.router.navigate([
            '/dashboard/interviews',
            interviewId,
            'questions'
          ]);
        },

        error: () => {
          this.loading = false;
          toast.error('Failed');
        }

      });
  }
}