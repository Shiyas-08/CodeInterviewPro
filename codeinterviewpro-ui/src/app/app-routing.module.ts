import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';
import { CreateInterviewComponent } from './features/interviews/pages/create-interview/create-interview.component';
import { AssignQuestionsComponent } from './features/interviews/pages/assign-questions/assign-questions.component';
import { ScheduleInterviewComponent } from './features/interviews/pages/schedule-interview/schedule-interview.component';
import { InviteCandidateComponent } from './features/interviews/pages/invite-candidate/invite-candidate.component';
import { InterviewRoomComponent } from './features/interviews/pages/interview-room/interview-room.component';
import { HomeComponent } from './features/home/pages/home/home.component';
import { InterviewMonitorComponent } from './features/dashboard/pages/interview-monitor/interview-monitor.component';
import { HrInterviewsComponent } from './features/dashboard/pages/hr-interviews/hr-interviews.component';
import { CandidateResultComponent } from './features/dashboard/pages/candidate-result/candidate-result.component';
import { QuestionBankComponent } from './features/dashboard/pages/question-bank/question-bank.component';
import { TenantManagementComponent } from './features/dashboard/pages/tenant-management/tenant-management.component';
import { CreateHrPageComponent } from './features/dashboard/pages/create-hr/create-hr.component';
import { ProfileComponent } from './features/dashboard/pages/profile/profile.component';
import { TenantDetailComponent } from './features/dashboard/pages/tenant-detail/tenant-detail.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2, 3] }
  },
  {
    path: 'admin/dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'interview',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'dashboard/interviews/create',
    component: CreateInterviewComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'dashboard/interviews/:id/questions',
    component: AssignQuestionsComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'dashboard/interviews/:id/schedule',
    component: ScheduleInterviewComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'dashboard/interviews/:id/invite',
    component: InviteCandidateComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'admin/interview-monitor/:id',
    component: InterviewMonitorComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'admin/hr-interviews',
    component: HrInterviewsComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'admin/questions',
    component: QuestionBankComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'admin/candidate-result/:candidateId/:interviewId',
    component: CandidateResultComponent,
    canActivate: [AuthGuard],
    data: { roles: [1, 2] }
  },
  {
    path: 'candidate/results/:interviewId',
    component: CandidateResultComponent,
    canActivate: [AuthGuard],
    data: { roles: [3] }
  },
  {
    path: 'admin/tenants',
    component: TenantManagementComponent,
    canActivate: [AuthGuard],
    data: { roles: [1] }
  },
  {
    path: 'admin/tenants/:id',
    component: TenantDetailComponent,
    canActivate: [AuthGuard],
    data: { roles: [1] }
  },
  {
    path: 'admin/create-hr',
    component: CreateHrPageComponent,
    canActivate: [AuthGuard],
    data: { roles: [1] }
  },
  {
    path: 'interview-room/:id',
    component: InterviewRoomComponent,
    canActivate: [AuthGuard],
    data: { roles: [3] }
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: ''
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
