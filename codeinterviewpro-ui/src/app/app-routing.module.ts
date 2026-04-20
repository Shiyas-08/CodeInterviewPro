import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';
import { CreateInterviewComponent } from './features/interviews/pages/create-interview/create-interview.component';
import { AssignQuestionsComponent } from './features/interviews/pages/assign-questions/assign-questions.component';
import { ScheduleInterviewComponent } from './features/interviews/pages/schedule-interview/schedule-interview.component';
import { InviteCandidateComponent } from './features/interviews/pages/invite-candidate/invite-candidate.component';
import { InterviewRoomComponent } from './features/interviews/pages/interview-room/interview-room.component';

const routes: Routes = [
  // ── Auth module (lazy loaded) ────────────────────────────
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then(m => m.AuthModule)
  },

  // ── HR Dashboard  (role 2 → /dashboard) ─────────────────
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },

  // ── Admin Dashboard  (role 1 → /admin/dashboard) ─────────
  {
    path: 'admin/dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },

  // ── Candidate Interview Room  (role 3 → /interview) ───────
  // Points to DashboardComponent for now; replace with
  // InterviewComponent once that module is built.
  {
    path: 'interview',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  {
  path: 'dashboard/interviews/create',
  component: CreateInterviewComponent,
  canActivate: [AuthGuard]
},
{
 path: 'dashboard/interviews/:id/questions',
 component: AssignQuestionsComponent,
 canActivate: [AuthGuard]
},
{
 path: 'dashboard/interviews/:id/schedule',
 component: ScheduleInterviewComponent,
 canActivate: [AuthGuard]
},
{
 path: 'dashboard/interviews/:id/invite',
 component: InviteCandidateComponent,
 canActivate: [AuthGuard]
},
{
  path: 'interview-room/:id',
  component: InterviewRoomComponent,
  canActivate: [AuthGuard]
},
  // ── Default & wildcard ───────────────────────────────────
  {
    path: '',
    redirectTo: 'auth/login',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
