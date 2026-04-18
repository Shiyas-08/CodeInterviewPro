// core/services/auth.service.ts

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(email: string, password: string, token?: string) {
    return this.http.post(`${this.baseUrl}/login`, {
      email,
      password,
      token
    }, { withCredentials: true });
  }

  register(fullName: string, email: string, password: string, token: string) {
    return this.http.post(`${this.baseUrl}/register`, {
      fullName,
      email,
      password,
      token
    }, { withCredentials: true });
  }

  me() {
    return this.http.get(`${this.baseUrl}/me`, {
      withCredentials: true
    });
  }

  logout() {
    return this.http.post(`${this.baseUrl}/logout`, {}, {
      withCredentials: true
    });
  }

  refresh() {
    return this.http.post(`${this.baseUrl}/refresh`, {}, {
      withCredentials: true
    });
  }
}