import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { toast } from '../services/toast';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Handled by AuthInterceptor mostly, but good to have a backup
        } else if (error.status === 403) {
          toast.error('Forbidden: You do not have permission to access this resource.');
          this.router.navigate(['/dashboard']);
        } else if (error.status === 500) {
          toast.error('Server Error: Something went wrong on our end. Please try again later.');
        } else if (error.status === 0) {
          toast.error('Network Error: Please check your internet connection.');
        } else {
          // For other errors, let the component handle or show a generic message
          const message = error.error?.message || error.statusText || 'An unexpected error occurred.';
          console.error('HTTP Error:', message);
        }

        return throwError(() => error);
      })
    );
  }
}
