// src/app/core/services/booking-api.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';
import { Package } from './travel-data';

export interface CreateBookingRequest {
  packageId:       string;
  departDate:      string;
  returnDate:      string;
  flightClass:     string;
  roomType:        string;
  couponCode?:     string;
  specialRequests?: string;
  travelers: Array<{ type: string; count: number }>;
  addons:    Array<{ name: string; price: number }>;
}

export interface BookingResponse {
  id:             string;
  bookingRef:     string;
  status:         string;
  totalAmount:    number;
  taxAmount:      number;
  discountAmount: number;
  couponCode:     string | null;
  departDate:     string;
  returnDate:     string;
  flightClass:    string;
  roomType:       string;
  createdAt:      string;
  package: {
    id:     string;
    name:   string;
    image:  string;
    nights: number;
  };
  travelers: Array<{ type: string; count: number }>;
  addons:    Array<{ name: string; price: number }>;
  payment:   null | {
    status: string;
    amount: number;
    method: string;
    paidAt: string | null;
  };
}

@Injectable({ providedIn: 'root' })
export class BookingService {
  private api = inject(ApiService);

  // ── Upgrade price maps ─────────────────────────────────
  readonly classUpgrades: Record<string, number> = {
    economy: 0,
    premium: 15000,
    business: 45000,
    first: 90000,
  };

  readonly roomUpgrades: Record<string, number> = {
    std: 0,
    deluxe: 8000,
    suite: 25000,
    villa: 60000,
  };

  // Signals for current booking state
  selectedPackage = signal<Package | null>(null);
  travelers = signal({ adults: 2, children: 0, infants: 0 });
  discount = signal<number>(0);
  couponApplied = signal(false);
  bookingMeta = signal<any>(null);

  setBookingMeta(data: any) {
    this.bookingMeta.set(data);
  }

  getBookingMeta() {
    return this.bookingMeta();
  }

  getTravelerArray() {
  const t = this.travelers();

  return [
    { type: 'adults', count: t.adults },
    { type: 'children', count: t.children },
    { type: 'infants', count: t.infants }
  ].filter(tr => tr.count > 0); // ✅ IMPORTANT
}

  // ── Create Booking ─────────────────────────────────────
  createBooking(req: CreateBookingRequest): Observable<BookingResponse> {
    return this.api.post<BookingResponse>('/bookings', req);
  }

  // ── Get My Bookings ────────────────────────────────────
  getMyBookings(): Observable<BookingResponse[]> {
    return this.api.get<BookingResponse[]>('/bookings');
  }

  // ── Get Single Booking ─────────────────────────────────
  getBookingById(id: string): Observable<BookingResponse> {
    return this.api.get<BookingResponse>(`/bookings/${id}`);
  }

  // ── Cancel Booking ─────────────────────────────────────
  cancelBooking(id: string): Observable<any> {
    return this.api.patch(`/bookings/${id}/cancel`, {});
  }

  // ── Validate Coupon ────────────────────────────────────
  validateCoupon(
    code: string,
    subtotal: number,
  ): Observable<{
    isValid: boolean;
    discountPct: number;
    message: string;
  }> {
    return this.api.post('/coupons/validate', { code, subtotal });
  }

  // ── Helpers ────────────────────────────────────────────
  changeTraveler(type: 'adults' | 'children' | 'infants', delta: number) {
    const current = this.travelers();
    const newVal = Math.max(type === 'adults' ? 1 : 0, current[type] + delta);
    this.travelers.set({ ...current, [type]: newVal });
  }

  getTotalPax(): number {
    const t = this.travelers();
    return t.adults + t.children;
  }
  getUpgradeTotal(classKey: string, roomKey: string): number {
    return (this.classUpgrades[classKey] ?? 0) + (this.roomUpgrades[roomKey] ?? 0);
  }
}
