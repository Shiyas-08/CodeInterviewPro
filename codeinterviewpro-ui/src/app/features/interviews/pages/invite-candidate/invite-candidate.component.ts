import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-invite-candidate',
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

  constructor(
    private route: ActivatedRoute,
    private service: InterviewService
  ) {
    this.interviewId =
      this.route.snapshot.paramMap.get('id') || '';
  }

  submit() {
    this.service.inviteCandidate(
      this.interviewId,
      this.form
    ).subscribe((res:any) => {

      this.inviteLink = res.data;

      alert('Invite Sent Successfully');
    });
  }
}