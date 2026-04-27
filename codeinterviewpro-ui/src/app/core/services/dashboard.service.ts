import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private baseUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getSummary() {
    return this.http.get(`${this.baseUrl}/summary`, {
      withCredentials: true
    });
  }

  getInsights() {
    return this.http.get(`${this.baseUrl}/insights`, {
      withCredentials: true
    });
  }

  getCandidateInterviews() {
    return this.http.get(`${environment.apiUrl}/candidate/interviews`, {
      withCredentials: true
    });
  }
}