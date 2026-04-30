import { toast } from 'src/app/core/services/toast';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-invite-candidate',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './invite-candidate.component.html'
})
export class InviteCandidateComponent {

  interviewId = '';
  inviteLink = '';

  form = {
    email: '',
    name: '',
    expiryTime: ''
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
    this.loading = true;
    this.service.inviteCandidate(
      this.interviewId,
      this.form
    ).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.inviteLink = res.data;
        toast.success('Invite Sent Successfully');
        this.router.navigate(['/admin/hr-interviews']);
      },
      error: (err) => {
        this.loading = false;
        toast.error('Failed to send invite');
      }
    });
  }
}