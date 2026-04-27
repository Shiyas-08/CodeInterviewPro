import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InterviewService {

  private baseUrl = `${environment.apiUrl}/interviews`;

  constructor(private http: HttpClient) {}

  createInterview(data: any): Observable<any> {
    return this.http.post(this.baseUrl, data);
  }

  getAllInterviews(): Observable<any> {
    return this.http.get(this.baseUrl);
  }

  assignQuestions(id: string, data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/questions`, data);
  }

  scheduleInterview(id: string, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}/schedule`, data);
  }

  inviteCandidate(id: string, data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/invite`, data);
  }
  getQuestions(): Observable<any> {
  return this.http.get(`${environment.apiUrl}/questions`);
}
startInterview(token: string): Observable<any> {
  return this.http.post(
    `${environment.apiUrl}/interview-execution/start`,
    { token }
  );
}


getSession(token: string) {
  return this.http.get(
   `${environment.apiUrl}/interview-session/get?token=${token}`
  );
}

stopSession(token: string) {
  return this.http.post(
    `${environment.apiUrl}/interview-session/stop?token=${token}`,
    {}
  );
}

deleteInterview(id: string): Observable<any> {
  return this.http.delete(`${this.baseUrl}/${id}`);
}
runCode(data:any){
  return this.http.post(
    `${environment.apiUrl}/interview-execution/run`,
    data
  );
}

submitInterview(data:any){
  return this.http.post(
    `${environment.apiUrl}/interview-execution/submit`,
    data
  );
}
resumeInterview(token: string) {
  return this.http.get(
    `${environment.apiUrl}/interview-session/resume?token=${token}`
  );
}

getCandidateResult(candidateId: string): Observable<any> {
  return this.http.get(`${environment.apiUrl}/results/candidate/${candidateId}`);
}
}