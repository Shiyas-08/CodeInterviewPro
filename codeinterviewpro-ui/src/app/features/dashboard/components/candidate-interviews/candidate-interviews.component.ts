import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardService } from 'src/app/core/services/dashboard.service';

@Component({
  selector: 'app-candidate-interviews',
  templateUrl: './candidate-interviews.component.html'
})
export class CandidateInterviewsComponent implements OnInit {

  interviews: any[] = [];
  loading = true;

  statusMap: any = {
    0: 'Assigned',
    1: 'Started',
    2: 'Completed',
    3: 'Expired'
  };

  constructor(
    private service: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadInterviews();
    
  }

  loadInterviews() {
    this.service.getCandidateInterviews().subscribe({
      next: (res: any) => {
        this.interviews = res;
        this.loading = false;
        this.interviews = res;
        console.log(res);
      },
      error: () => {
        this.loading = false;
      }
    });
  }

joinInterview(token: string) {
  this.router.navigate(['/interview-room', token]);
}
}