import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Core
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';

// Shared
import { NavbarComponent } from './shared/navbar/navbar.component';
import { ToasterComponent } from './shared/components/toaster/toaster.component';
import { HlmSpinnerComponent } from './shared/components/spinner/spinner.component';

// Features - Dashboard
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { CandidateInterviewsComponent } from './features/dashboard/components/candidate-interviews/candidate-interviews.component';
import { InterviewMonitorComponent } from './features/dashboard/pages/interview-monitor/interview-monitor.component';
import { HrInterviewsComponent } from './features/dashboard/pages/hr-interviews/hr-interviews.component';
import { CandidateResultComponent } from './features/dashboard/pages/candidate-result/candidate-result.component';
import { TenantListComponent } from './features/dashboard/components/tenant-list/tenant-list.component';
import { CreateTenantComponent } from './features/dashboard/components/create-tenant/create-tenant.component';
import { CreateHrComponent } from './features/dashboard/components/create-hr/create-hr.component';

// Features - Interviews
import { CreateInterviewComponent } from './features/interviews/pages/create-interview/create-interview.component';
import { AssignQuestionsComponent } from './features/interviews/pages/assign-questions/assign-questions.component';
import { ScheduleInterviewComponent } from './features/interviews/pages/schedule-interview/schedule-interview.component';
import { InviteCandidateComponent } from './features/interviews/pages/invite-candidate/invite-candidate.component';
import { InterviewRoomComponent } from './features/interviews/pages/interview-room/interview-room.component';

// Features - Home
import { HomeComponent } from './features/home/pages/home/home.component';
import { QuestionBankComponent } from './features/dashboard/pages/question-bank/question-bank.component';
import { TenantManagementComponent } from './features/dashboard/pages/tenant-management/tenant-management.component';
import { ProfileComponent } from './features/dashboard/pages/profile/profile.component';
import { TenantDetailComponent } from './features/dashboard/pages/tenant-detail/tenant-detail.component';
import { CreateHrPageComponent } from './features/dashboard/pages/create-hr/create-hr.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MonacoEditorModule.forRoot(),
    NavbarComponent,
    ToasterComponent,
    HlmSpinnerComponent,
    DashboardComponent,
    CandidateInterviewsComponent,
    InterviewMonitorComponent,
    HrInterviewsComponent,
    CandidateResultComponent,
    TenantListComponent,
    CreateTenantComponent,
    CreateHrComponent,
    AssignQuestionsComponent,
    ScheduleInterviewComponent,
    InviteCandidateComponent,
    InterviewRoomComponent,
    HomeComponent,
    QuestionBankComponent,
    TenantManagementComponent,
    ProfileComponent,
    TenantDetailComponent,
    CreateHrPageComponent,
    CreateInterviewComponent
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
