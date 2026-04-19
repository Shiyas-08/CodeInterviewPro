import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { AuthGuard } from './core/guards/auth.guard';

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
