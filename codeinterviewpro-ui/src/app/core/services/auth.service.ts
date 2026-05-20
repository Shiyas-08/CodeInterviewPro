// core/services/auth.service.ts

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Try to load user from localStorage on startup
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        this.currentUserSubject.next(JSON.parse(storedUser));
      } catch (e) {}
    }
  }

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
    }).pipe(
      tap((res: any) => {
        const user = res.data || res.Data || res;
        this.currentUserSubject.next(user);
        localStorage.setItem('user', JSON.stringify(user));
      })
    );
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

  forgotPassword(email: string) {
    return this.http.post(`${this.baseUrl}/forgot-password`, { email });
  }

  resetPassword(data: any) {
    return this.http.post(`${this.baseUrl}/reset-password`, data);
  }
}