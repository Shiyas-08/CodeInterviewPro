import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import { toast } from 'src/app/core/services/toast';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  email: string = '';
  token: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  loading: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.token = params['token'];
      
      if (!this.email || !this.token) {
        toast.error('Invalid reset link');
        this.router.navigate(['/auth/login']);
      }
    });
  }

  resetPassword() {
    if (this.newPassword.length < 6) {
      toast.error('Password must be at least 6 characters');
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      toast.error('Passwords do not match');
      return;
    }

    this.loading = true;
    const data = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword
    };

    this.authService.resetPassword(data).subscribe({
      next: (res: any) => {
        this.loading = false;
        toast.success('Password reset successful! Please login.');
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.loading = false;
        toast.error(err.error?.message || 'Error resetting password');
      }
    });
  }
}
