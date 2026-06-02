// src/app/core/services/auth.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api';

export interface AuthUser {
  id:          string;
  name:        string;
  email:       string;
  phone:       string;
  role:        string;
  nationality: string | null;
  dob:         string | null;
  createdAt:   string;
}

export interface AuthResponse {
  accessToken:  string;
  refreshToken: string;
  user:         AuthUser;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api    = inject(ApiService);
  private router = inject(Router);

  currentUser = signal<AuthUser | null>(this.loadUser());

  // ── Register ──────────────────────────────────────────
  register(data: {
    name: string; email: string;
    phone: string; password: string; nationality?: string;
  }): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('/auth/register', data).pipe(
      tap(res => this.saveSession(res))
    );
  }

  // ── Login ─────────────────────────────────────────────
  login(email: string, password: string): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('/auth/login', { email, password }).pipe(
      tap(res => this.saveSession(res))
    );
  }

  // ── Logout ────────────────────────────────────────────
  logout(): void {
    this.api.post('/auth/logout', {}).subscribe({ error: () => {} });
    this.clearSession();
  }

  // ── Get My Profile ────────────────────────────────────
  getMe(): Observable<AuthUser> {
    return this.api.get<AuthUser>('/users/me').pipe(
      tap(user => {
        this.currentUser.set(user);
        localStorage.setItem('user', JSON.stringify(user));
      })
    );
  }

  // ── Update Profile ────────────────────────────────────
  updateMe(data: {
    name?: string; phone?: string;
    nationality?: string; dob?: string;
  }): Observable<AuthUser> {
    return this.api.patch<AuthUser>('/users/me', data).pipe(
      tap(user => {
        this.currentUser.set(user);
        localStorage.setItem('user', JSON.stringify(user));
      })
    );
  }

  // ── Change Password ───────────────────────────────────
  changePassword(data: {
    currentPassword: string;
    newPassword: string;
  }): Observable<{ message: string }> {
    return this.api.post<{ message: string }>(
      '/users/me/change-password', data
    );
  }

  // ── Delete Account ────────────────────────────────────
  deleteMe(): Observable<void> {
    return this.api.delete<void>('/users/me').pipe(
      tap(() => this.clearSession())
    );
  }

  // ── Forgot / Reset Password ───────────────────────────
  forgotPassword(email: string): Observable<{ message: string }> {
    return this.api.post('/auth/forgot-password', { email });
  }

  resetPassword(email: string, otp: string, newPassword: string) {
    return this.api.post('/auth/reset-password', { email, otp, newPassword });
  }

  // ── Helpers ───────────────────────────────────────────
  isLoggedIn(): boolean { return !!localStorage.getItem('access_token'); }
  isAdmin():    boolean { return this.currentUser()?.role === 'Admin'; }
  getToken():   string | null { return localStorage.getItem('access_token'); }

  private saveSession(res: AuthResponse) {
    localStorage.setItem('access_token',  res.accessToken);
    localStorage.setItem('refresh_token', res.refreshToken);
    localStorage.setItem('user',          JSON.stringify(res.user));
    this.currentUser.set(res.user);
  }

  private clearSession() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.router.navigate(['/home']);
  }

  private loadUser(): AuthUser | null {
    try {
      const u = localStorage.getItem('user');
      return u ? JSON.parse(u) : null;
    } catch { return null; }
  }
}

// // src/app/core/services/auth.service.ts
// import { Injectable, inject, signal } from '@angular/core';
// import { Router } from '@angular/router';
// import { tap } from 'rxjs/operators';
// import { ApiService } from './api';

// export interface AuthUser {
//   id:          string;
//   name:        string;
//   email:       string;
//   phone:       string;
//   role:        string;
//   nationality: string | null;
//   dob:         string | null;
//   createdAt:   string;
// }

// export interface AuthResponse {
//   accessToken:  string;
//   refreshToken: string;
//   user:         AuthUser;
// }

// @Injectable({ providedIn: 'root' })
// export class AuthService {
//   private api    = inject(ApiService);
//   private router = inject(Router);

//   // Reactive current user signal
//   currentUser = signal<AuthUser | null>(this.loadUserFromStorage());

//   // ── Register ───────────────────────────────────────────
//   register(data: {
//     name:        string;
//     email:       string;
//     phone:       string;
//     password:    string;
//     nationality?: string;
//   }) {
//     return this.api.post<AuthResponse>('/auth/register', data).pipe(
//       tap(res => this.saveSession(res))
//     );
//   }

//   // ── Login ──────────────────────────────────────────────
//   login(email: string, password: string) {
//     return this.api.post<AuthResponse>('/auth/login', { email, password }).pipe(
//       tap(res => this.saveSession(res))
//     );
//   }

//   // ── Logout ─────────────────────────────────────────────
//   logout() {
//     return this.api.post('/auth/logout', {}).pipe(
//       tap(() => this.clearSession())
//     );
//   }

//   // ── Forgot Password ────────────────────────────────────
//   forgotPassword(email: string) {
//     return this.api.post('/auth/forgot-password', { email });
//   }

//   // ── Reset Password ─────────────────────────────────────
//   resetPassword(email: string, otp: string, newPassword: string) {
//     return this.api.post('/auth/reset-password', { email, otp, newPassword });
//   }

//   // ── Helpers ────────────────────────────────────────────
//   // isLoggedIn(): boolean {
//   //   return !!localStorage.getItem('access_token');
//   // }
//   isLoggedIn = signal(!!localStorage.getItem('access_token'));
  

//   isAdmin(): boolean {
//     return this.currentUser()?.role === 'Admin';
//   }

//   getToken(): string | null {
//     return localStorage.getItem('access_token');
//   }

//   private saveSession(res: AuthResponse) {
//     localStorage.setItem('access_token',  res.accessToken);
//     localStorage.setItem('refresh_token', res.refreshToken);
//     localStorage.setItem('user',          JSON.stringify(res.user));
//     this.currentUser.set(res.user);
//     this.isLoggedIn.set(true); 
//   }

//   private clearSession() {
//     localStorage.removeItem('access_token');
//     localStorage.removeItem('refresh_token');
//     localStorage.removeItem('user');
//     this.currentUser.set(null);
//     this.isLoggedIn.set(false); 
//     this.router.navigate(['/']);
//   }

//   private loadUserFromStorage(): AuthUser | null {
//     try {
//       const u = localStorage.getItem('user');
//       return u ? JSON.parse(u) : null;
//     } catch {
//       return null;
//     }
//   }
// }
