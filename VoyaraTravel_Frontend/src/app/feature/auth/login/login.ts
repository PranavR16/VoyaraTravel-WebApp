import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {

  // ── Tab state ──────────────────────────────────────────
  activeTab:  'login' | 'register' | 'forgot' | 'profile' = 'login';
  profileTab: 'info' | 'password' | 'danger' = 'info';

  // ── Shared state ───────────────────────────────────────
  loading     = signal(false);
  errorMsg    = signal('');
  successMsg  = signal('');
  showPwd     = false;

  // ── Login fields ───────────────────────────────────────
  email    = signal('');
  password = signal('');

  // ── Register fields ────────────────────────────────────
  regName        = signal('');
  regEmail       = signal('');
  regPhone       = signal('');
  regPassword    = signal('');
  regConfirm     = signal('');
  regNationality = signal('');

  // ── Forgot/Reset ───────────────────────────────────────
  otpSent     = signal(false);
  otp         = signal('');
  newPassword = signal('');

  // ── Profile fields ─────────────────────────────────────
  profileName        = signal('');
  profilePhone       = signal('');
  profileNationality = signal('');
  profileDob         = signal('');
  currentPwd         = signal('');
  newPwd             = signal('');
  confirmPwd         = signal('');

  // ── Delete ─────────────────────────────────────────────
  confirmDelete     = false;
  deleteConfirmText = '';

  constructor(
    public  authService: AuthService,
    private router:      Router,
    private route:       ActivatedRoute
  ) {}

  ngOnInit() {
    // If already logged in → show profile
    if (this.authService.isLoggedIn()) {
      this.activeTab = 'profile';
      this.loadProfileFields();
      return;
    }

    // Support ?tab=register query param
    this.route.queryParams.subscribe(p => {
      if (p['tab'] === 'register') this.activeTab = 'register';
    });
  }

  user() { return this.authService.currentUser(); }

  switchTab(tab: typeof this.activeTab) {
    this.activeTab = tab;
    this.errorMsg.set('');
    this.successMsg.set('');
  }

  // ── Login ─────────────────────────────────────────────
  onLogin() {
    if (!this.email() || !this.password()) {
      this.errorMsg.set('Please enter email and password');
      return;
    }
    this.loading.set(true);
    this.errorMsg.set('');

    this.authService.login(this.email(), this.password()).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/home']);
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Invalid email or password');
      }
    });
  }

  // ── Register ──────────────────────────────────────────
  onRegister() {
    if (!this.regName() || !this.regEmail() || !this.regPhone() || !this.regPassword()) {
      this.errorMsg.set('Please fill in all required fields');
      return;
    }
    if (this.regPassword() !== this.regConfirm()) {
      this.errorMsg.set('Passwords do not match');
      return;
    }
    if (this.regPassword().length < 8) {
      this.errorMsg.set('Password must be at least 8 characters');
      return;
    }

    this.loading.set(true);
    this.errorMsg.set('');

    this.authService.register({
      name:        this.regName(),
      email:       this.regEmail(),
      phone:       this.regPhone(),
      password:    this.regPassword(),
      nationality: this.regNationality() || undefined
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/home']);
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Registration failed');
      }
    });
  }

  // ── Forgot Password ───────────────────────────────────
  onForgotPassword() {
    if (!this.email()) {
      this.errorMsg.set('Please enter your email');
      return;
    }
    this.loading.set(true);
    this.errorMsg.set('');

    this.authService.forgotPassword(this.email()).subscribe({
      next: () => {
        this.loading.set(false);
        this.otpSent.set(true);
        this.successMsg.set('OTP sent to your email');
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Email not found');
      }
    });
  }

  onResetPassword() {
    if (!this.otp() || !this.newPassword()) {
      this.errorMsg.set('Please enter OTP and new password');
      return;
    }
    this.loading.set(true);

    this.authService.resetPassword(this.email(), this.otp(), this.newPassword()).subscribe({
      next: () => {
        this.loading.set(false);
        this.successMsg.set('Password reset successfully!');
        setTimeout(() => this.switchTab('login'), 1500);
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Invalid OTP');
      }
    });
  }

  // ── Profile ───────────────────────────────────────────
  private loadProfileFields() {
    const u = this.user();
    if (!u) return;
    this.profileName.set(u.name || '');
    this.profilePhone.set(u.phone || '');
    this.profileNationality.set(u.nationality || '');
    this.profileDob.set(u.dob ? u.dob.split('T')[0] : '');
  }

  onUpdateProfile() {
    this.loading.set(true);
    this.errorMsg.set('');
    this.successMsg.set('');

    this.authService.updateMe({
      name:        this.profileName(),
      phone:       this.profilePhone(),
      nationality: this.profileNationality(),
      dob:         this.profileDob() || undefined
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.successMsg.set('Profile updated successfully!');
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Update failed');
      }
    });
  }

  onChangePassword() {
    if (!this.currentPwd() || !this.newPwd()) {
      this.errorMsg.set('Please fill in all fields');
      return;
    }
    if (this.newPwd() !== this.confirmPwd()) {
      this.errorMsg.set('New passwords do not match');
      return;
    }
    if (this.newPwd().length < 8) {
      this.errorMsg.set('Password must be at least 8 characters');
      return;
    }

    this.loading.set(true);
    this.errorMsg.set('');

    this.authService.changePassword({
      currentPassword: this.currentPwd(),
      newPassword:     this.newPwd()
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.successMsg.set('Password changed successfully!');
        this.currentPwd.set('');
        this.newPwd.set('');
        this.confirmPwd.set('');
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Incorrect current password');
      }
    });
  }

  onDeleteAccount() {
    if (this.deleteConfirmText !== 'DELETE') return;
    this.loading.set(true);

    this.authService.deleteMe().subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/home']);
      },
      error: err => {
        this.loading.set(false);
        this.errorMsg.set(err?.error?.message || 'Delete failed');
      }
    });
  }

  onLogout() {
    this.authService.logout();
  }
}

// import { Component, signal } from '@angular/core';
// import { AuthService } from '../../../core/services/auth.service';
// import { Router } from '@angular/router';
// import { CommonModule } from '@angular/common';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-login',
//   imports:[CommonModule,FormsModule],
//   standalone: true,
//   templateUrl: './login.html',
//   styleUrls: ['./login.scss']
// })
// export class Login {

//   email = signal('');
//   password = signal('');
//   errorMessage = signal('');

//   constructor(
//     private authService: AuthService,
//     private router: Router
//   ) {}

//   onLogin() {
//     this.errorMessage.set('');

//     this.authService.login(this.email(), this.password()).subscribe({
//       next: () => {
//         this.router.navigate(['/home']);
//       },
//       error: (err) => {
//         this.errorMessage.set(err?.error?.message || 'Invalid credentials');
//       }
//     });
//   }
// }