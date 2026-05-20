import { Component } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { toast } from 'src/app/core/services/toast';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email: string = '';
  loading: boolean = false;
  emailSent: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  sendResetLink() {
    if (!this.email) {
      toast.error('Please enter your email address');
      return;
    }

    this.loading = true;
    this.authService.forgotPassword(this.email).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.emailSent = true;
        toast.success('Reset link sent to your email');
      },
      error: (err) => {
        this.loading = false;
        toast.error(err.error?.message || 'Error sending reset link');
      }
    });
  }
}
