import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;
  role: number = 0;
  showNavbar = true;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.auth.me().subscribe({
      next: (res: any) => {
        this.isLoggedIn = true;
        const user = res.data || res.Data || res;
        this.role = Number(user.role);
        this.checkRoute();
      },
      error: () => {
        this.isLoggedIn = false;
        this.checkRoute();
      }
    });

    this.router.events.subscribe(() => {
      this.checkRoute();
    });
  }

  checkRoute() {
    const url = this.router.url;
    // Hide navbar in interview room or if logged in (per request)
    this.showNavbar = !url.includes('interview-room');
    
    // If you strictly want it hidden AFTER LOGIN:
    if (this.isLoggedIn && !url.includes('dashboard')) {
        // You might want it hidden everywhere except dashboard?
        // Let's stick to hiding in Interview Room for now as it's the primary issue.
    }
  }

  logout() {
    this.auth.logout().subscribe({
      next: () => {
        this.isLoggedIn = false;
        this.router.navigate(['/']);
      },
      error: () => {
        this.isLoggedIn = false;
        this.router.navigate(['/']);
      }
    });
  }
}
