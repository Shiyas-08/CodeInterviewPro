import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-candidate-interviews',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './candidate-interviews.component.html'
})
export class CandidateInterviewsComponent implements OnInit {

  interviews: any[] = [];
  loading = true;

  constructor(
    private dashboardService: DashboardService,
    private interviewService: InterviewService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadInterviews();
  }

  loadInterviews(): void {
    this.loading = true;

    this.dashboardService.getCandidateInterviews().subscribe({
      next: (res: any) => {
        this.interviews = res || [];
        this.loading = false;

        console.log('Candidate Interviews:', this.interviews);
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.interviews = [];
      }
    });
  }

  getPendingCount(): number {
    return this.interviews.filter(x => x.status === 'Pending').length;
  }

  getButtonText(status: string): string {

    if (status === 'Pending') {
      return 'Start Interview';
    }

    if (status === 'InProgress') {
      return 'Resume Interview';
    }

    if (status === 'Completed') {
      return 'View Result';
    }

    if (status === 'Expired') {
      return 'Expired';
    }

    return 'Open Interview';
  }

  joinInterview(item: any): void {

    if (item.status === 'Expired') {
      return;
    }

    if (item.status === 'Completed') {
      this.router.navigate(['/candidate/results', item.interviewId]);
      return;
    }

    // Resume existing session
    if (item.status === 'InProgress') {
      this.router.navigate(['/interview-room', item.token]);
      return;
    }

    // Pending -> Start API call first
    this.loading = true;

    this.interviewService.startInterview(item.token).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/interview-room', item.token]);
      },
      error: (err) => {
        this.loading = false;
        console.error(err);
        toast.error('Unable to start interview');
      }
    });
  }
}