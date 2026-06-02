import { Component, OnInit, ViewEncapsulation   } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BookingService } from '../../core/services/booking';
import { ToastService } from '../../core/services/toast';
import { Package } from '../../core/services/travel-data';

interface Addon {
  icon: string;
  name: string;
  price: number;
  selected: boolean;
}

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking.html',
  styleUrl: './booking.scss',
  encapsulation: ViewEncapsulation.None
})
export class Booking implements OnInit {

  // Steps
  steps = ['Travelers', 'Personal Info', 'Travel Dates'];
  currentStep = 0;
 
  // Coupon
  couponCode = '';
  couponApplied = false;
  couponError = false;
  discountPct = 0;
  pending_coupon=sessionStorage.getItem('pending_coupon');
  
  // Form
  form = {
    firstName: '', lastName: '',
    email: '', phone: '',
    dob: '', nationality: 'Indian',
    passport: '', passportExpiry: '',
    specialRequests: '',
    departDate: '', returnDate: '',
    flightClass: 'economy',
    roomType: 'std'
  };

  // Validation errors
  errors: Record<string, boolean> = {};

  // Traveler types
  travelerTypes = [
    { key: 'adults',   label: 'Adults',   age: 'Age 12+ years',   icon: '🧑' },
    { key: 'children', label: 'Children', age: 'Age 2–11 years',  icon: '👦' },
    { key: 'infants',  label: 'Infants',  age: 'Age under 2 yrs', icon: '👶' },
  ];

  // Add-ons
  addons: Addon[] = [
    { icon: '🛡', name: 'Travel Insurance',    price: 2999,  selected: false },
    { icon: '🚗', name: 'Airport Transfer',    price: 1499,  selected: false },
    { icon: '📸', name: 'Photography Tour',    price: 4999,  selected: false },
    { icon: '🍽', name: 'Gourmet Dining',      price: 3499,  selected: false },
    { icon: '🧘', name: 'Spa & Wellness',      price: 5999,  selected: false },
    { icon: '🤿', name: 'Water Sports',        price: 3999,  selected: false },
  ];

  // Upgrades map
  private classUpgrades: Record<string, number> = {
    economy: 0, premium: 15000, business: 45000, first: 90000
  };
  private roomUpgrades: Record<string, number> = {
    std: 0, deluxe: 8000, suite: 25000, villa: 60000
  };
  private coupons: Record<string, number> = {
    VOYARA10: 10, SUMMER20: 20, FIRST15: 15
  };

  constructor(
    public bookingService: BookingService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    if (!this.pkg) {
      this.toastService.error('Please select a package first');
      this.router.navigate(['/packages']);
    }
    if(this.pending_coupon)
    {
      this.couponApplied=true;
      this.couponCode=this.pending_coupon;
      this.applyCoupon();
    }
  }
  
  // ── Package ──
  get pkg(): Package | null {
    return this.bookingService.selectedPackage();
  }

  // ── Progress ──
  get progressWidth(): string {
    return `${(this.currentStep / (this.steps.length - 1)) * 100}%`;
  }

  // ── Travelers ──
  getTravelerCount(key: string): number {
    const t = this.bookingService.travelers();
    return t[key as keyof typeof t];
  }

  changeTraveler(key: string, delta: number) {
    this.bookingService.changeTraveler(
      key as 'adults' | 'children' | 'infants', delta
    );
  }

  get totalPax(): number {
    const t = this.bookingService.travelers();
    return t.adults + t.children;
  }

  // ── Pricing ──
  get classUpgradePrice(): number {
    return this.classUpgrades[this.form.flightClass] || 0;
  }

  get roomUpgradePrice(): number {
    return this.roomUpgrades[this.form.roomType] || 0;
  }

  get addonsTotal(): number {
    return this.addons
      .filter(a => a.selected)
      .reduce((sum, a) => sum + a.price, 0);
  }

  get subtotal(): number {
    if (!this.pkg) return 0;
    return (
      (this.pkg.price + this.classUpgradePrice + this.roomUpgradePrice)
      * this.totalPax
    ) + this.addonsTotal;
  }

  get taxAmount(): number {
    return Math.round(this.subtotal * 0.05);
  }

  get discountAmount(): number 
  {
    return this.couponApplied
      ? Math.round(this.subtotal * this.discountPct / 100)
      : 0;
  }

  get grandTotal(): number {
    return this.subtotal + this.taxAmount - this.discountAmount;
  }

  recalculate() {
    // getter-based — auto recalculates
  }

  // ── Add-ons ──
  toggleAddon(addon: Addon) {
    addon.selected = !addon.selected;
    const msg = addon.selected
      ? `✓ ${addon.name} added`
      : `${addon.name} removed`;
    this.toastService.info(msg);
  }
  // ── Coupon ──
  applyCoupon() {
    this.couponError = false;
    const pct = this.coupons[this.couponCode.toUpperCase()];
    if (pct) {
      this.discountPct = pct;
      this.couponApplied = true;
      this.toastService.success(`🎉 ${pct}% discount applied!`);
    } else {
      this.couponError = true;
      setTimeout(() => this.couponError = false, 3000);
      this.toastService.error('Invalid coupon code');
    }
  }

  // ── Navigation ──
  nextStep() {
    if (this.currentStep < this.steps.length - 1) {
      this.currentStep++;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  prevStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  // ── Validation ──
  validateAndNext() {
    this.errors = {};
    if (!this.form.firstName)  this.errors['firstName'] = true;
    if (!this.form.lastName)   this.errors['lastName']  = true;
    if (!this.form.email || !this.form.email.includes('@'))
                               this.errors['email']     = true;
    if (!this.form.phone)      this.errors['phone']     = true;

    if (Object.keys(this.errors).length > 0) {
      this.toastService.error('Please fill all required fields');
      return;
    }
    this.nextStep();
  }

  validateDatesAndNext() {
    this.errors = {};
    if (!this.form.departDate) this.errors['departDate'] = true;
    if (!this.form.returnDate) this.errors['returnDate'] = true;

    if (Object.keys(this.errors).length > 0) {
      this.toastService.error('Please select travel dates');
      return;
    }
    this.nextStep();
  }

  // ── Go to payment ──
  // goToPayment() {
  //   this.router.navigate(['/payment']);
  //   this.toastService.success('Almost there! Complete your payment.');
  // }
  goToPayment() {

  const bookingMeta = {
    departDate: this.form.departDate,
    returnDate: this.form.returnDate,
    flightClass: this.form.flightClass,
    roomType: this.form.roomType,
    couponCode: this.couponCode || null,
    specialRequests: this.form.specialRequests || null,
    addons: this.addons
      .filter(a => a.selected)
      .map(a => ({
        name: a.name,
        price: a.price
      }))
  };

  this.bookingService.setBookingMeta(bookingMeta); // ✅
  this.router.navigate(['/payment']);
  this.toastService.success('Almost there! Complete your payment.');
}
}