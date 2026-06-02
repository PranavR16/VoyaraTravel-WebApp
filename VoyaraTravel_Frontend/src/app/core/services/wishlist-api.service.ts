// src/app/core/services/wishlist-api.service.ts
import { Injectable, inject, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { ApiService } from './api';

export interface WishlistItem {
  id:           string;
  packageId:    string;
  packageName:  string;
  packageImage: string;
  packagePrice: number;
  createdAt:    string;
}

@Injectable({ providedIn: 'root' })
export class WishlistApiService {
  private api = inject(ApiService);

  // Local signal to track wishlisted IDs
  wishlistedIds = signal<Set<string>>(new Set());

  // ── Get Wishlist ───────────────────────────────────────
  getWishlist() {
    return this.api.get<WishlistItem[]>('/wishlist').pipe(
      tap(items => {
        const ids = new Set(items.map(i => i.packageId));
        this.wishlistedIds.set(ids);
      })
    );
  }

  // ── Add to Wishlist ────────────────────────────────────
  add(packageId: string) {
    return this.api.post(`/wishlist/${packageId}`, {}).pipe(
      tap(() => {
        const current = new Set(this.wishlistedIds());
        current.add(packageId);
        this.wishlistedIds.set(current);
      })
    );
  }

  // ── Remove from Wishlist ───────────────────────────────
  remove(packageId: string) {
    return this.api.delete(`/wishlist/${packageId}`).pipe(
      tap(() => {
        const current = new Set(this.wishlistedIds());
        current.delete(packageId);
        this.wishlistedIds.set(current);
      })
    );
  }

  // ── Toggle ─────────────────────────────────────────────
  toggle(packageId: string) {
    if (this.wishlistedIds().has(packageId))
      return this.remove(packageId);
    return this.add(packageId);
  }

  isWishlisted(packageId: string): boolean {
    return this.wishlistedIds().has(packageId);
  }
}
