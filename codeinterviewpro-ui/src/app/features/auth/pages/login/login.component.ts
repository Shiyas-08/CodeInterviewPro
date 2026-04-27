import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  email = '';
  password = '';
  token: string | null = null;
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
  }

  login() {
    if (this.loading) return;
    this.loading = true;
    this.auth.login(this.email, this.password, this.token || undefined)
      .subscribe({
        next: () => {
          this.loading = false;
          toast.success('Login successful! Welcome back.');
          this.loadUser();
        },
        error: () => {
          this.loading = false;
          toast.error('Login failed. Please check your credentials.');
        }
      });
  }

  loadUser() {
    this.auth.me().subscribe({
      next: (user: any) => {
        const role = Number(user.role);
        if (role === 1) {
          this.router.navigate(['/admin/dashboard']);
        } else if (role === 2) {
          this.router.navigate(['/dashboard']);
        } else if (role === 3) {
          this.router.navigate(['/dashboard']);
        }
      }
    });
  }
}