import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResultService {
  private baseUrl = `${environment.apiUrl}/results`;

  constructor(private http: HttpClient) {}

  getMyResult(interviewId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/my/${interviewId}`);
  }

  getCandidateResult(candidateId: string, interviewId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/candidate/${candidateId}/interview/${interviewId}`);
  }
}
