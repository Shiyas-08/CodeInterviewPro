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

  constructor(
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    //  Read token from URL
    this.token = this.route.snapshot.queryParamMap.get('token');

    if (!this.token) {
      alert('Invalid invite link');
      this.router.navigate(['/auth/login']);
    }
  }

 register() {
  if (!this.token) return;

  this.auth.register(this.fullName, this.email, this.password, this.token)
    .subscribe({
      next: (res: any) => {
        console.log('REGISTER SUCCESS RESPONSE:', res); // 🔥 ALWAYS LOG

        if (res?.success === true) {
          alert(res.message || 'Registration successful');

          this.router.navigate(['/auth/login'], {
            queryParams: { token: this.token }
          });
        } else {
          // backend returned 200 but success = false
          alert(res?.message || 'Registration failed');
        }
      },

      error: (err) => {
        console.log('REGISTER ERROR RESPONSE:', err); // 🔥 ALWAYS LOG

        const message =
          err?.error?.message ||   // ApiResponse
          err?.error ||            // string
          'Registration failed';

        alert(message);
      }
    });
}
}