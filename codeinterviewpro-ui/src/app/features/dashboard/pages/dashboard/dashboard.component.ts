import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { InterviewService } from 'src/app/core/services/interview.service';
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
  recentInterviews: any[] = [];

  monitorToken = '';

  constructor(
    private dashboardService: DashboardService,
    private auth: AuthService,
    private signalrService: SignalrService,
    private interviewService: InterviewService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadUser();
    this.loadSummary();

    this.signalrService.startConnection().then(() => {
      this.signalrService.interviewSubmitted$.subscribe(() => {
        this.loadSummary();
        if (this.role === 2) this.loadRecentInterviews();
      });
    });
  }

  loadUser() {
    this.auth.currentUser$.subscribe(user => {
      if (user) {
        this.userProfile = user;
        const rawRole = user.role ?? user.Role ?? user.userRole ?? user.UserRole;
        this.role = Number(rawRole);
        if (this.role === 2) {
          this.loadRecentInterviews();
        }
      }
    });
  }

  loadRecentInterviews() {
    this.interviewService.getAllInterviews().subscribe({
      next: (res: any) => {
        const all = res.data || res;
        // Sort: Live (3) first, then by date descending
        this.recentInterviews = all.sort((a: any, b: any) => {
          if (a.status === 3 && b.status !== 3) return -1;
          if (a.status !== 3 && b.status === 3) return 1;
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        }).slice(0, 5);
      }
    });
  }

  loadSummary() {
    this.loading = true;
    this.error = false;
    this.dashboardService.getSummary().subscribe({
      next: (res: any) => {
        this.summary = res.data || res.Data || res;
        this.loading = false;
        // Load insights for everyone (Personal for Candidate, Aggregate for HR/Admin)
        this.loadInsights();
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
        console.log('DEBUG DASHBOARD INSIGHTS:', this.insights);
        this.insightsLoading = false;
      },
      error: (err) => {
        console.error('Insights Load Error:', err);
        this.insightsLoading = false;
      }
    });
  }

  refreshDashboard() {
    this.router.navigate(['/dashboard']).then(() => {
      this.loadSummary();
      if (this.role === 2) this.loadRecentInterviews();
      console.log('Dashboard Refreshed');
    });
  }

  monitorSession() {
    if (this.monitorToken.trim()) {
      window.location.href = '/admin/interview-monitor/' + this.monitorToken.trim();
    }
  }

  logout() {
    this.auth.logout().subscribe(() => {
      localStorage.clear();
      window.location.href = '/auth/login';
    });
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

  get scoreDashArray(): number {
    return 2 * Math.PI * 42;
  }

  get scoreDashOffset(): number {
    const circumference = this.scoreDashArray;
    return circumference - (this.scorePercent / 100) * circumference;
  }
}