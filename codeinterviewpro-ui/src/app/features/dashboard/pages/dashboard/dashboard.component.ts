import { Component, OnInit } from '@angular/core';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {

  summary: any;
  role: number = 0;

  constructor(
    private dashboardService: DashboardService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    this.loadUser();
    this.loadSummary();
  }

  loadUser() {
    this.auth.me().subscribe((res: any) => {
      this.role = Number(res.role);
    });
  }

  loadSummary() {
    this.dashboardService.getSummary().subscribe((res: any) => {
      console.log('Dashboard Summary:', res);
      this.summary = res;
    });
  }
}