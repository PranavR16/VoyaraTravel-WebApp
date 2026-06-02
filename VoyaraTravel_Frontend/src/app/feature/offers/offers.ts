import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TravelDataService, Package } from '../../core/services/travel-data';
import { ApiService } from '../../core/services/api';
import { BookingService } from '../../core/services/booking';

interface Coupon {
  code:        string;
  discount:    number;
  label:       string;
  description: string;
  icon:        string;
  color:       string;
  expiry:      string;
  copied:      boolean;
}

@Component({
  selector: 'app-offers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './offers.html',
  styleUrl:    './offers.scss'
})
export class Offers implements OnInit, OnDestroy {

  // ── Countdown ──────────────────────────────────────────
  days    = signal(0);
  hours   = signal(0);
  minutes = signal(0);
  seconds = signal(0);
  private timer: any;

  // Target: 7 days from now
  private targetDate = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);

  // ── Coupon Validator ───────────────────────────────────
  couponInput   = '';
  validating    = false;
  couponResult  = signal<{ valid: boolean; msg: string; pct: number } | null>(null);

  // ── Packages ───────────────────────────────────────────
  topDeals$!: Observable<Package[]>;

  // ── Coupons ────────────────────────────────────────────
  coupons: Coupon[] = [
    {
      code:        'VOYARA10',
      discount:    10,
      label:       'Welcome Offer',
      description: 'Get 10% off on any package. Perfect for first-time bookers!',
      icon:        '🌿',
      color:       'green',
      expiry:      'Valid for 1 year',
      copied:      false
    },
    {
      code:        'SUMMER20',
      discount:    20,
      label:       'Summer Special',
      description: 'Our biggest seasonal discount. Book any beach or resort package.',
      icon:        '☀️',
      color:       'gold',
      expiry:      'Valid for 6 months',
      copied:      false
    },
    {
      code:        'FIRST15',
      discount:    15,
      label:       'First Booking',
      description: 'Exclusive 15% off on your very first booking with Voyara.',
      icon:        '✈️',
      color:       'blue',
      expiry:      'Valid for 1 year',
      copied:      false
    }
  ];

  // ── Flash Deals ────────────────────────────────────────
  flashDeals = [
    {
      icon:  '🏖',
      title: 'Beach Escapes',
      off:   'Up to 30% off',
      desc:  'Bali, Maldives & Goa packages'
    },
    {
      icon:  '🏔',
      title: 'Adventure Trails',
      off:   'Up to 25% off',
      desc:  'Nepal, Bhutan & Swiss packages'
    },
    {
      icon:  '❤',
      title: 'Honeymoon Suites',
      off:   'Up to 20% off',
      desc:  'Santorini, Paris & Bali packages'
    },
    {
      icon:  '👨‍👩‍👧',
      title: 'Family Fun',
      off:   'Kids stay free',
      desc:  'Select family-friendly resorts'
    },
    {
      icon:  '💎',
      title: 'Luxury Collection',
      off:   'Free upgrade',
      desc:  'Suite upgrade on select hotels'
    },
    {
      icon:  '🌏',
      title: 'Last Minute',
      off:   'Save up to 40%',
      desc:  'Departures within 7 days'
    }
  ];

  constructor(
    private dataService:    TravelDataService,
    private api:            ApiService,
    private bookingService: BookingService,
    public  router:         Router
  ) {}

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'instant' });
    this.startCountdown();

    // Top deals = packages sorted by highest discount, take 6
    this.topDeals$ = this.dataService.getPackages().pipe(
      map(pkgs =>
        [...pkgs]
          .sort((a, b) => (b.discount ?? 0) - (a.discount ?? 0))
          .slice(0, 6)
      )
    );
  }

  ngOnDestroy() {
    clearInterval(this.timer);
  }

  // ── Countdown Timer ────────────────────────────────────
  private startCountdown() {
    this.tick();
    this.timer = setInterval(() => this.tick(), 1000);
  }

  private tick() {
    const now  = Date.now();
    const diff = Math.max(0, this.targetDate.getTime() - now);

    this.days.set(Math.floor(diff / 86400000));
    this.hours.set(Math.floor((diff % 86400000) / 3600000));
    this.minutes.set(Math.floor((diff % 3600000) / 60000));
    this.seconds.set(Math.floor((diff % 60000) / 1000));
  }

  // ── Copy Coupon ────────────────────────────────────────
  copyCoupon(coupon: Coupon) {
    navigator.clipboard.writeText(coupon.code).then(() => {
      coupon.copied = true;
      setTimeout(() => coupon.copied = false, 2000);
    });
  }

  // ── Validate Coupon ────────────────────────────────────
  validateCoupon() {
    if (!this.couponInput.trim()) return;
    this.validating = true;
    this.couponResult.set(null);

    // POST /api/coupons/validate
    this.api.post<{ isValid: boolean; discountPct: number; message: string }>(
      '/coupons/validate',
      { code: this.couponInput.toUpperCase(), subtotal: 100000 }
    ).subscribe({
      next: res => {
        this.validating = false;
        this.couponResult.set({
          valid: res.isValid,
          msg:   res.message,
          pct:   res.discountPct
        });
      },
      error: () => {
        this.validating = false;
        this.couponResult.set({
          valid: false,
          msg:   'Could not validate coupon. Please try again.',
          pct:   0
        });
      }
    });
  }

  // ── Book with Coupon ───────────────────────────────────
  bookWithCoupon(code: string) {
    // Store coupon in session for booking page to pick up
    sessionStorage.setItem('pending_coupon', code);
    this.router.navigate(['/packages']);
  }

  onBookPackage(pkg: Package) {
    this.bookingService.selectedPackage.set(pkg);
    this.router.navigate(['/booking']);
  }

  // ── Pad number ─────────────────────────────────────────
  pad(n: number): string {
    return n.toString().padStart(2, '0');
  }
}
