// package-card.component.ts
import {  Component, Input, Output, EventEmitter, OnInit,ViewEncapsulation} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Package } from '../../core/services/travel-data';
import { BookingService } from '../../core/services/booking';
import { ToastService } from '../../core/services/toast';
import { WishlistApiService } from '../../core/services/wishlist-api.service';
@Component({
  selector: 'app-package-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './package-card.html',
  styleUrl: './package-card.scss',
  encapsulation: ViewEncapsulation.Emulated   // ← ADD THIS
})
export class PackageCard implements OnInit {

  @Input() pkg!: Package;
  @Output() bookClicked = new EventEmitter<Package>();

  constructor(
    private bookingService: BookingService,
    private toastService: ToastService,
    private router: Router,
    private wishlist:WishlistApiService
  ) {}

  ngOnInit() {
    if (!this.pkg) {
      console.error('PackageCardComponent: pkg input is required');
    }
  }

  get isWishlisted(): boolean {
    return this.wishlist.isWishlisted(this.pkg.id);
  }

  get starsDisplay(): string {
    const filled = '★'.repeat(this.pkg.rating);
    const empty  = '☆'.repeat(5 - this.pkg.rating);
    return filled + empty;
  }

  onWishlistToggle(event: Event) {
    event.stopPropagation();
    const added = this.wishlist.toggle(this.pkg.id);
    if (added) {
      this.toastService.success(`❤ ${this.pkg.name} added to wishlist!`);
    } else {
      this.toastService.error(`Removed from wishlist`);
    }
  }

  onBook() {
    this.bookingService.selectedPackage.set(this.pkg);
    if (this.bookClicked.observed) {
      this.bookClicked.emit(this.pkg);
    } else {
      this.router.navigate(['/booking']);
    }
  }
}
