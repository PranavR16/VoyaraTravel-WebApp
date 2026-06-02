// navbar.ts — verify these are correct
import { Component, HostListener, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit {
  isScrolled  = false;
  mobileOpen  = false;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    // Refresh user from API on app load if logged in
    if (this.authService.isLoggedIn()) {
      this.authService.getMe().subscribe({ error: () => {} });
    }
  }

  isLoggedIn() { return this.authService.isLoggedIn(); }
  user()       { return this.authService.currentUser(); }

  @HostListener('window:scroll')
  onScroll() { this.isScrolled = window.scrollY > 40; }

  toggleMobile() { this.mobileOpen = !this.mobileOpen; }

  goToLogin() { this.router.navigate(['/login']); }

  logout() {
    this.authService.logout();
    this.mobileOpen = false;
  }
}

// import { Component, HostListener, computed, inject } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { Router, RouterModule } from '@angular/router';
// import { AuthService } from '../../core/services/auth.service';

// @Component({
//   selector: 'app-navbar',
//   standalone: true,
//   imports: [CommonModule, RouterModule],
//   templateUrl: './navbar.html',
//   styleUrls: ['./navbar.scss']
// })
// export class Navbar {

//   // ✅ Inject services using inject()
//   private authService = inject(AuthService);
//   private router = inject(Router);

//   // UI State
//   isScrolled = false;
//   mobileOpen = false;

//   // Auth State
//   // isLoggedIn = computed(() => this.authService.isLoggedIn());
//   isLoggedIn = this.authService.isLoggedIn;
//   user = this.authService.currentUser;

//   // Scroll Effect
//   @HostListener('window:scroll')
//   onScroll() {
//     this.isScrolled = window.scrollY > 60;
//   }

//   toggleMobile() {
//     this.mobileOpen = !this.mobileOpen;
//   }

//   goToLogin() {
//     this.router.navigate(['/login']);
//   }

//   logout() {
//     this.authService.logout().subscribe();
//   }
// }