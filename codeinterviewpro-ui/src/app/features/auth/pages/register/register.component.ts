import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  fullName = '';
  email = '';
  password = '';
  token: string | null = null;
  loading = false;

  constructor(
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
  }

  register() {
    if (this.loading) return;

    this.loading = true;
    this.auth.register(this.fullName, this.email, this.password, this.token || '')
      .subscribe({
        next: (res: any) => {
          this.loading = false;
          // Handle both raw string responses and JSON objects
          const isSuccess = typeof res === 'string' || res?.success || res?.Success;
          const message = typeof res === 'string' ? res : (res?.message || 'Success');

          if (isSuccess) {
            toast.success(message);
            this.router.navigate(['/auth/login'], { queryParams: { token: this.token } });
          } else {
            toast.error(message || 'Registration failed');
          }
        },
        error: (err) => {
          this.loading = false;
          // Handle cases where the backend returns a string but Angular fails to parse it as JSON
          const message = err?.error?.text || err?.error?.message || err?.error || 'Registration failed';
          
          if (typeof err?.error === 'string' && err.error.includes('successfully')) {
             toast.success(err.error);
             this.router.navigate(['/auth/login'], { queryParams: { token: this.token } });
          } else {
             toast.error(typeof message === 'string' ? message : 'Registration failed');
          }
        }
      });
  }
}