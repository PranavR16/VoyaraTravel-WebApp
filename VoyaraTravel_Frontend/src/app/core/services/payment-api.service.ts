// src/app/core/services/payment-api.service.ts
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api';

export interface CreateOrderResponse {
  razorpayOrderId: string;
  amount:          number;
  currency:        string;
  keyId:           string;
}

export interface VerifyPaymentRequest {
  razorpayOrderId:   string;
  razorpayPaymentId: string;
  razorpaySignature: string;
  bookingId:         string;
}

export interface PaymentResult {
  success:    boolean;
  bookingRef: string;
  message:    string;
}

@Injectable({ providedIn: 'root' })
export class PaymentApiService {
  private api = inject(ApiService);

  // Step 1 — Create Razorpay order on backend
  //bookingId: string, method: string
  createOrder(data: { bookingId: string; method: string }): Observable<CreateOrderResponse> {
    return this.api.post<CreateOrderResponse>(
      '/payments/create-order',
      data
    );
  }

  // Step 2 — Verify payment after Razorpay success callback
  verifyPayment(req: VerifyPaymentRequest): Observable<PaymentResult> {
    return this.api.post<PaymentResult>('/payments/verify', req);
  }

  // Helper: Open Razorpay checkout modal
  openRazorpay(orderData: CreateOrderResponse, bookingId: string): Promise<PaymentResult> {
    return new Promise((resolve, reject) => {
      const options = {
        key:         orderData.keyId,
        amount:      orderData.amount * 100,  // paise
        currency:    orderData.currency,
        order_id:    orderData.razorpayOrderId,
        name:        'Voyara Travel',
        description: 'Luxury Travel Booking',
        image:       '/assets/logo.png',
        theme:       { color: '#1B4332' },

        handler: (response: any) => {
          // Verify on backend after success
          this.verifyPayment({
            razorpayOrderId:   response.razorpay_order_id,
            razorpayPaymentId: response.razorpay_payment_id,
            razorpaySignature: response.razorpay_signature,
            bookingId
          }).subscribe({
            next:  res   => resolve(res),
            error: err   => reject(err)
          });
        },

        modal: {
          ondismiss: () => reject(new Error('Payment cancelled by user'))
        }
      };

      // Load Razorpay script dynamically
      const script = document.createElement('script');
      script.src   = 'https://checkout.razorpay.com/v1/checkout.js';
      script.onload = () => {
        const rzp = new (window as any).Razorpay(options);
        rzp.open();
      };
      document.body.appendChild(script);
    });
  }
}
