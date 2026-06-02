import {Component,OnInit,ViewEncapsulation} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BookingService } from '../../core/services/booking';
import { ToastService } from '../../core/services/toast';
import { Package } from '../../core/services/travel-data';
import { PaymentApiService } from '../../core/services/payment-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payment.html',
  styleUrl: './payment.scss',
  encapsulation: ViewEncapsulation.None,
})
export class Payment implements OnInit {
  // ── State ──
  activeMethod = 'card';
  processing = false;
  paymentSuccess = false;
  bookingEmail = '';
  bookingRef = '';

  // ── Card form ──
  cardFlipped = false;
  saveCard = false;
  cardForm = {
    number: '',
    name: '',
    expiry: '',
    cvv: '',
  };
  cardErrors: Record<string, boolean> = {};

  // ── EMI ──
  showEmi = false;
  selectedEmi = 0;
  emiPlans = [
    { months: 3, rate: 12 },
    { months: 6, rate: 14 },
    { months: 12, rate: 16 },
  ];

  // ── UPI ──
  upiId = '';
  upiError = false;
  selectedUpiApp = '';
  upiApps = [
    { id: 'gpay', icon: '🟢', name: 'GPay' },
    { id: 'phonepe', icon: '🟣', name: 'PhonePe' },
    { id: 'paytm', icon: '🔵', name: 'Paytm' },
    { id: 'bhim', icon: '🟠', name: 'BHIM' },
  ];

  // ── Net Banking ──
  selectedBank = '';
  banks = [
    { id: 'sbi', icon: '🏦', name: 'SBI' },
    { id: 'hdfc', icon: '🏦', name: 'HDFC' },
    { id: 'icici', icon: '🏦', name: 'ICICI' },
    { id: 'axis', icon: '🏦', name: 'Axis' },
  ];
  allBanks = [
    { id: 'kotak', name: 'Kotak Bank' },
    { id: 'pnb', name: 'Punjab National' },
    { id: 'bob', name: 'Bank of Baroda' },
    { id: 'canara', name: 'Canara Bank' },
    { id: 'union', name: 'Union Bank' },
    { id: 'idfc', name: 'IDFC First Bank' },
    { id: 'yes', name: 'Yes Bank' },
    { id: 'indusind', name: 'IndusInd Bank' },
  ];

  // ── Wallets ──
  selectedWallet = '';
  wallets = [
    { id: 'paytm', icon: '💙', name: 'Paytm Wallet' },
    { id: 'amazon', icon: '🟡', name: 'Amazon Pay' },
    { id: 'mobikwik', icon: '💜', name: 'MobiKwik' },
    { id: 'freecharge', icon: '🔶', name: 'FreeCharge' },
  ];

  // ── Payment methods ──
  methods = [
    { id: 'card', icon: '💳', label: 'Card' },
    { id: 'upi', icon: '📱', label: 'UPI' },
    { id: 'netbanking', icon: '🏛', label: 'Net Banking' },
    { id: 'wallet', icon: '👛', label: 'Wallet' },
  ];

  constructor(
    public bookingService: BookingService,
    private toastService: ToastService,
    public router: Router,
    private paymentService: PaymentApiService,
    // public authService:AuthService
  ) {}

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'instant' });
    if (!this.pkg) {
      this.toastService.error('Please select a package first');
      this.router.navigate(['/packages']);
    }
  }

  // ── Package ──
  get pkg(): Package | null {
    return this.bookingService.selectedPackage();
  }

  // ── Travelers ──
  get totalPax(): number {
    const t = this.bookingService.travelers();
    return t.adults + t.children;
  }

  // ── Pricing ──
  get baseTotal(): number {
    return (this.pkg?.price || 0) * this.totalPax;
  }

  // ── Methods ──
  setMethod(id: string) {
    this.activeMethod = id;
    this.cardErrors = {};
  }

  // ── Card formatting ──
  get displayCardNumber(): string {
    const n = this.cardForm.number.replace(/\s/g, '');
    if (!n) return '•••• •••• •••• ••••';
    const padded = n.padEnd(16, '•');
    return padded.match(/.{1,4}/g)?.join(' ') || padded;
  }

  get cardBrand(): string {
    const n = this.cardForm.number.replace(/\s/g, '');
    if (n.startsWith('4')) return 'VISA';
    if (n.startsWith('5')) return 'MASTERCARD';
    if (n.startsWith('6')) return 'RUPAY';
    if (n.startsWith('37')) return 'AMEX';
    return 'CARD';
  }

  formatCardNumber(event: Event) {
    const input = event.target as HTMLInputElement;
    let val = input.value.replace(/\D/g, '').slice(0, 16);
    this.cardForm.number = val.match(/.{1,4}/g)?.join(' ') || val;
    input.value = this.cardForm.number;
  }

  formatExpiry(event: Event) {
    const input = event.target as HTMLInputElement;
    let val = input.value.replace(/\D/g, '').slice(0, 4);
    if (val.length >= 3) {
      val = val.slice(0, 2) + '/' + val.slice(2);
    }
    this.cardForm.expiry = val;
    input.value = val;
  }

  // ── Validate Card ──
  private validateCard(): boolean {
    this.cardErrors = {};
    const num = this.cardForm.number.replace(/\s/g, '');
    if (num.length !== 16) this.cardErrors['number'] = true;
    if (!this.cardForm.name.trim()) this.cardErrors['name'] = true;
    if (this.cardForm.expiry.length !== 5) this.cardErrors['expiry'] = true;
    if (this.cardForm.cvv.length < 3) this.cardErrors['cvv'] = true;
    return Object.keys(this.cardErrors).length === 0;
  }

  // ── Process Payment ──
  processPayment() {
    // ✅ Step 1: Validate (keep your existing validation)

    if (this.activeMethod === 'card' && !this.validateCard()) {
      this.toastService.error('Please fill in all card details');
      return;
    }

    if (this.activeMethod === 'upi' && !this.upiId.includes('@') && !this.selectedUpiApp) {
      this.upiError = true;
      this.toastService.error('Please enter a valid UPI ID');
      return;
    }

    if (this.activeMethod === 'netbanking' && !this.selectedBank) {
      this.toastService.error('Please select your bank');
      return;
    }

    if (this.activeMethod === 'wallet' && !this.selectedWallet) {
      this.toastService.error('Please select a wallet');
      return;
    }

    this.processing = true;
    this.toastService.info('🔒 Processing your payment...');

    // ✅ Step 2: CREATE BOOKING FIRST
    const meta = this.bookingService.getBookingMeta();
    if (!meta || !this.pkg) {
      this.toastService.error('Booking data missing');
      this.router.navigate(['/booking']);
      return;
    }

    const bookingReq = {
      packageId: this.pkg.id,
      departDate: meta.departDate,
      returnDate: meta.returnDate,
      flightClass: meta.flightClass,
      roomType: meta.roomType,
      couponCode: meta.couponCode,
      specialRequests: meta.specialRequests,
      travelers: this.bookingService.getTravelerArray(), // ✅ FIXED
      addons: meta.addons,
    };

    this.bookingService.createBooking(bookingReq).subscribe({
      next: (bookingRes) => {
        const bookingId = bookingRes.id; // ✅ IMPORTANT

        // ✅ Step 3: CREATE ORDER
        this.paymentService
          .createOrder({
            bookingId: bookingId,
            method: this.activeMethod,
          })
          .subscribe({
            next: (orderRes) => {
              // 🧪 Dummy mode
              if (orderRes.keyId === 'dummy_key') {
                setTimeout(() => {
                  const fakeVerify = {
                    razorpayOrderId: orderRes.razorpayOrderId,
                    razorpayPaymentId: 'pay_dummy_' + Date.now(),
                    razorpaySignature: 'dummy_signature',
                  };

                  this.verifyPayment(fakeVerify);
                }, 1500);
              } else {
                // future real razorpay
              }
            },
            error: () => {
              this.processing = false;
              this.toastService.error('Failed to create payment order');
            },
          });
      },
      error: () => {
        this.processing = false;
        this.toastService.error('Booking creation failed');
      },
    });
  }

  verifyPayment(data: any) {
    this.paymentService.verifyPayment(data).subscribe({
      next: (res) => {
        this.processing = false;
        this.paymentSuccess = true;
        this.bookingRef = res.bookingRef; // ✅ from backend

        window.scrollTo({ top: 0, behavior: 'instant' });

        this.toastService.success('🎉 Payment successful! Booking confirmed.');
      },
      error: () => {
        this.processing = false;
        this.toastService.error('Payment verification failed');
      },
    });
  }

  get upgradeTotal(): number {
    const meta = this.bookingService.getBookingMeta?.();
    if (!meta) return 0;

    return this.bookingService.getUpgradeTotal(meta.flightClass, meta.roomType);
  }

  get addonsTotal(): number {
    const meta = this.bookingService.getBookingMeta?.();
    if (!meta?.addons) return 0;

    return meta.addons.reduce((sum: number, a: any) => sum + a.price, 0);
  }

  get subtotal(): number {
    return this.baseTotal + this.upgradeTotal + this.addonsTotal;
  }

  get taxAmount(): number {
    return Math.round(this.subtotal * 0.05);
  }

  get discountAmount(): number {
    const discount = this.bookingService.discount?.() || 0;
    return discount ? Math.round((this.subtotal * discount) / 100) : 0;
  }
  get grandTotal(): number {
    return this.subtotal + this.taxAmount - this.discountAmount;
  }
}