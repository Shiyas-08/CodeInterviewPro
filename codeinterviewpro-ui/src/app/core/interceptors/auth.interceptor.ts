import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    // auto include cookies
    const clonedReq = req.clone({
      withCredentials: true
    });

    return next.handle(clonedReq).pipe(

      catchError((error: HttpErrorResponse) => {

        if (error.status === 401) {
          console.log('Unauthorized → redirect login');
          this.router.navigate(['/auth/login']);
        }

        if (error.status === 403) {
          alert('Access denied');
        }

        if (error.status === 500) {
          alert('Server error occurred');
        }

        return throwError(() => error);
      })

    );
  }
}