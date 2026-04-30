import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { toast } from '../services/toast';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    return new Promise((resolve) => {

      this.auth.me().subscribe({
        next: (res: any) => {
          const user = res.data || res.Data || res;
          const role = Number(user.role);
          const requiredRoles = route.data['roles'] as Array<number>;

          if (requiredRoles && !requiredRoles.includes(role)) {
            toast.warning('Access Denied: You do not have the required permissions.');
            this.router.navigate(['/dashboard']);
            resolve(false);
          } else {
            resolve(true);
          }
        },
        error: () => {
          // Check for token in query params or route params (for /interview/:id)
          const token = route.queryParamMap.get('token') || route.paramMap.get('id');
          
          if (token) {
            this.router.navigate(['/auth/login'], { queryParams: { token } });
          } else {
            this.router.navigate(['/auth/login']);
          }
          resolve(false);
        }
      });

    });
  }
}