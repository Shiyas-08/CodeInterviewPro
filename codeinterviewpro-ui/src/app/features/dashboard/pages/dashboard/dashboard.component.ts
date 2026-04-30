import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { CandidateInterviewsComponent } from '../../components/candidate-interviews/candidate-interviews.component';
import { TenantListComponent } from '../../components/tenant-list/tenant-list.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, CandidateInterviewsComponent, TenantListComponent],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {

  summary: any;
  insights: any;
  role: number = 0;
  userProfile: any;
  loading = true;
  insightsLoading = false;
  error = false;

  monitorToken = '';

  constructor(
    private dashboardService: DashboardService,
    private auth: AuthService,
    private signalrService: SignalrService
  ) {}

  ngOnInit() {
    this.loadUser();
    this.loadSummary();

    this.signalrService.startConnection().then(() => {
      this.signalrService.interviewSubmitted$.subscribe(() => {
        this.loadSummary();
      });
    });
  }

  loadUser() {
    this.auth.me().subscribe((res: any) => {
      this.userProfile = res.data || res.Data || res;
      this.role = Number(this.userProfile.role);
    });
  }

  loadSummary() {
    this.loading = true;
    this.error = false;
    this.dashboardService.getSummary().subscribe({
      next: (res: any) => {
        this.summary = res.data || res.Data || res;
        this.loading = false;
        // Only load insights for Admin (1) and HR (2)
        if (this.role !== 3) {
          this.loadInsights();
        }
      },
      error: (err) => {
        console.error('Dashboard Error:', err);
        this.error = true;
        this.loading = false;
      }
    });
  }

  loadInsights() {
    this.insightsLoading = true;
    this.dashboardService.getInsights().subscribe({
      next: (res: any) => {
        this.insights = res.data || res.Data || res;
        this.insightsLoading = false;
      },
      error: () => {
        this.insightsLoading = false;
      }
    });
  }

  monitorSession() {
    if (this.monitorToken.trim()) {
      window.location.href = '/admin/interview-monitor/' + this.monitorToken.trim();
    }
  }

  get scorePercent(): number {
    if (!this.insights?.averageScore) return 0;
    return Math.min(Math.round(this.insights.averageScore), 100);
  }

  get scoreColor(): string {
    const s = this.scorePercent;
    if (s >= 75) return '#10b981'; // emerald
    if (s >= 50) return '#6366f1'; // indigo
    return '#f43f5e';              // rose
  }

  get scoreDashOffset(): number {
    const circumference = 2 * Math.PI * 40;
    return circumference - (this.scorePercent / 100) * circumference;
  }
}