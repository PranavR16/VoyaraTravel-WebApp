// // src/app/core/interceptors/auth.interceptor.ts
// import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
// import { inject } from '@angular/core';
// import { Router } from '@angular/router';
// import { catchError, throwError } from 'rxjs';

// export const authInterceptor: HttpInterceptorFn = (req, next) => {
//   const router = inject(Router);
//   const token  = localStorage.getItem('access_token');

//   // Attach token to every request
//   const authReq = token
//     ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
//     : req;

//   return next(authReq).pipe(
//     catchError((error: HttpErrorResponse) => {
//       // Auto-logout on 401
//       if (error.status === 401) {
//         localStorage.removeItem('access_token');
//         localStorage.removeItem('refresh_token');
//         router.navigate(['/login']);
//       }
//       return throwError(() => error);
//     })
//   );
// };
// src/app/core/interceptors/auth.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('access_token');

  // 🚨 Skip auth for public APIs
  const isPublicApi =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/forgot-password') ||
    req.url.includes('/auth/reset-password');

  // ✅ Attach token only if NOT public API
  const authReq = (!isPublicApi && token)
    ? req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {

      // 🚨 Handle 401 safely
      if (error.status === 401 && !isPublicApi) {

        // Clear session
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('user');

        // Avoid redirect loop
        if (router.url !== '/login') {
          router.navigate(['/login']);
        }
      }

      return throwError(() => error);
    })
  );
};