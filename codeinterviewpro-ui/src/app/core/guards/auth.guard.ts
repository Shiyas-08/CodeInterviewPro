import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  canActivate(): Promise<boolean> {
    return new Promise((resolve) => {

      this.auth.me().subscribe({
        next: () => {
          // ✅ user logged in
          resolve(true);
        },
        error: () => {
          // ❌ not logged in
          this.router.navigate(['/auth/login']);
          resolve(false);
        }
      });

    });
  }
}