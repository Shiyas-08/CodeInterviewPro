import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'codeinterviewpro-ui';
  showNavbar = true;
  showSidebar = false;
  role: number = 0;
  userProfile: any;

  constructor(private router: Router, private auth: AuthService) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      const url = event.urlAfterRedirects;
      
      this.showNavbar = !url.includes('/dashboard') &&
                        !url.includes('/interview-room') && 
                        !url.includes('/admin/interview-monitor') &&
                        !url.includes('/admin/hr-interviews') &&
                        !url.includes('/admin/questions') &&
                        !url.includes('/auth') &&
                        !url.includes('/candidate/results') &&
                        !url.includes('/admin/candidate-result');

      this.showSidebar = (
        url.includes('/dashboard') || 
        url.includes('/admin') || 
        url.includes('/profile')
      ) && !url.includes('/interview-room') && !url.includes('/auth');
    });

    // Reactive User State (The Real Fix)
    this.auth.currentUser$.subscribe(user => {
      if (user) {
        this.userProfile = user;
        const rawRole = user.role ?? user.Role ?? user.userRole ?? user.UserRole;
        this.role = Number(rawRole);
        console.log('Global Sidebar Role:', this.role);
      } else {
        this.role = 0;
        this.userProfile = null;
      }
    });
  }

  ngOnInit() {
    this.auth.me().subscribe(); 
  }

  logout() {
    this.auth.logout().subscribe(() => {
      localStorage.clear();
      window.location.href = '/auth/login';
    });
  }
}
