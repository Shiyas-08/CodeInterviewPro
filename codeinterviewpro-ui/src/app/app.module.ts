import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { DashboardComponent } from './features/dashboard/pages/dashboard/dashboard.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { CreateInterviewComponent } from './features/interviews/pages/create-interview/create-interview.component';
import { AssignQuestionsComponent } from './features/interviews/pages/assign-questions/assign-questions.component';
import { ScheduleInterviewComponent } from './features/interviews/pages/schedule-interview/schedule-interview.component';
import { InviteCandidateComponent } from './features/interviews/pages/invite-candidate/invite-candidate.component';
import { CandidateInterviewsComponent } from './features/dashboard/components/candidate-interviews/candidate-interviews.component';
import { InterviewRoomComponent } from './features/interviews/pages/interview-room/interview-room.component';
import { MonacoEditorModule } from 'ngx-monaco-editor-v2';
@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    AssignQuestionsComponent,
    ScheduleInterviewComponent,
    InviteCandidateComponent,
    CandidateInterviewsComponent,
    InterviewRoomComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MonacoEditorModule.forRoot()
  ],
  providers: [{provide:HTTP_INTERCEPTORS,useClass:AuthInterceptor,multi:true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
