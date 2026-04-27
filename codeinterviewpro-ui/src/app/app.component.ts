import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'codeinterviewpro-ui';
  showNavbar = true;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      const url = event.urlAfterRedirects;
      // Hide navbar in interview room, monitor, HR interview list, auth, and results pages
      this.showNavbar = !url.includes('/interview-room') && 
                        !url.includes('/admin/interview-monitor') &&
                        !url.includes('/admin/hr-interviews') &&
                        !url.includes('/admin/questions') &&
                        !url.includes('/auth') &&
                        !url.includes('/candidate/results') &&
                        !url.includes('/admin/candidate-result');
    });
  }
}
