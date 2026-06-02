// src/app/core/services/travel-data.ts
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api';   // ✅ matches your import path

export interface Package {
  id:              string;
  name:            string;
  description:     string;
  price:           number;
  oldPrice:        number;
  nights:          number;
  category:        string;
  badge:           string;
  badgeClass:      string;
  rating:          number;
  reviewCount:     number;
  reviews:         string;   // ✅ kept for template compatibility
  discount:        number;
  image:           string;
  unit:            string;
  tags:            string[];
  inclusions:      string[];
  destinationId:   string;
  destinationName: string;
}

export interface Destination {
  id:            string;
  name:          string;
  country:       string;
  image:         string;
  startingPrice: number;
  description:   string | null;
  packageCount:  number;
  big?:          boolean;   // ✅ frontend-only — not sent by backend
}

export interface PackageFilter {
  category?: string;
  sortBy?:   string;
  search?:   string;
  page?:     number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class TravelDataService {
  private api = inject(ApiService);

  // ── Testimonials (mock — no backend endpoint needed) ──────────
  testimonials = [
    {
      text:   'Voyara transformed our Maldives honeymoon into an absolutely magical experience. Every detail was perfectly curated — from the seaplane to the overwater villa.',
      name:   'Priya Sharma',
      loc:    '⭐⭐⭐⭐⭐ · Mumbai, India',
      avatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&q=80'
    },
    {
      text:   "The Japan Cherry Blossom package was beyond our expectations. Our guide was incredibly knowledgeable and Voyara's 24/7 support made everything stress-free.",
      name:   'Rahul Gupta',
      loc:    '⭐⭐⭐⭐⭐ · Delhi, India',
      avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&q=80'
    },
    {
      text:   'As a solo traveler, Voyara made me feel safe throughout my Europe tour. The experiences were truly once-in-a-lifetime!',
      name:   'Ananya Krishnan',
      loc:    '⭐⭐⭐⭐⭐ · Bangalore, India',
      avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&q=80'
    },
  ];

  // ── Packages ──────────────────────────────────────────────────
  getPackages(filter?: PackageFilter): Observable<Package[]> {
    return this.api.get<Package[]>('/packages', filter as any).pipe(
      // ✅ reviewCount (number from backend) → reviews (string for template)
      map(pkgs => pkgs.map(p => ({
        ...p,
        reviews: p.reviewCount ? p.reviewCount.toLocaleString('en-IN') : ''
      })))
    );
  }

  getPackageById(id: string): Observable<Package> {
    return this.api.get<Package>(`/packages/${id}`).pipe(
      map(p => ({
        ...p,
        reviews: p.reviewCount ? p.reviewCount.toLocaleString('en-IN') : ''
      }))
    );
  }

  getPackagesByCategory(category: string): Observable<Package[]> {
    if (!category || category === 'all')
      return this.getPackages();
    return this.api.get<Package[]>(`/packages/category/${category}`).pipe(
      map(pkgs => pkgs.map(p => ({
        ...p,
        reviews: p.reviewCount ? p.reviewCount.toLocaleString('en-IN') : ''
      })))
    );
  }

  // ── Destinations ──────────────────────────────────────────────
  getDestinations(): Observable<Destination[]> {
    return this.api.get<Destination[]>('/destinations').pipe(
      // ✅ Mark first destination as 'big' for the hero grid card
      map(dests => dests.map((d, i) => ({
        ...d,
        big: i === 0
      })))
    );
  }

  getDestinationById(id: string): Observable<Destination> {
    return this.api.get<Destination>(`/destinations/${id}`);
  }
}