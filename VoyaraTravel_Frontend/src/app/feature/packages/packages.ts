import { Component,OnInit,ViewEncapsulation} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Package, TravelDataService } from '../../core/services/travel-data';
import { BookingService } from '../../core/services/booking';
import { PackageCard } from '../../components/package-card/package-card';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-packages',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, PackageCard],
  templateUrl: './packages.html',
  styleUrl: './packages.scss',
  //encapsulation: ViewEncapsulation.None  // ← add
})
export class Packages implements OnInit {

  activeFilter = 'all';
  sortBy = 'popular';

  filters = [
    { label: 'All',           value: 'all'       },
    { label: '🏖 Beach',      value: 'beach'     },
    { label: '⛰ Adventure',   value: 'adventure' },
    { label: '🏛 Cultural',    value: 'cultural'  },
    { label: '❤ Honeymoon',   value: 'honeymoon' },
    { label: '👨‍👩‍👧 Family', value: 'family'    },
    { label: '💎 Luxury',     value: 'luxury'    },
  ];

  sortOptions = [
    { label: 'Most Popular',    value: 'popular'  },
    { label: 'Price: Low–High', value: 'low'      },
    { label: 'Price: High–Low', value: 'high'     },
    { label: 'Top Rated',       value: 'rating'   },
    { label: 'Duration',        value: 'duration' },
    { label: 'Discount',        value: 'discount' },
  ];

  constructor(
    public dataService: TravelDataService,
    private bookingService: BookingService,
    public router: Router
  ) {}
  filteredPackages$!: Observable<Package[]>;

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'instant' });
    this.loadPackages(); // ✅
    //  this.filteredPackages$ = this.dataService
    // .getPackagesByCategory(this.activeFilter)
    // .pipe(
    //   map(pkgs => {
    //     switch (this.sortBy) {
    //       case 'low':      return [...pkgs].sort((a, b) => a.price - b.price);
    //       case 'high':     return [...pkgs].sort((a, b) => b.price - a.price);
    //       case 'rating':   return [...pkgs].sort((a, b) => b.rating - a.rating);
    //       case 'duration': return [...pkgs].sort((a, b) => b.nights - a.nights);
    //       case 'discount': return [...pkgs].sort((a, b) => (b.discount || 0) - (a.discount || 0));
    //       default:         return pkgs;
    //     }
    //   })
    // );
  }

  setFilter(cat: string) {
    //this.activeFilter = cat;
    if (this.activeFilter === cat) return;
      this.activeFilter = cat;
      this.loadPackages(); // ✅ ADD THIS
  }

  onBookPackage(pkg: any) {
    this.bookingService.selectedPackage.set(pkg);
    this.router.navigate(['/booking']);
  }

  loadPackages() {
  this.filteredPackages$ = this.dataService
    .getPackagesByCategory(this.activeFilter)
    .pipe(
      map(pkgs => {
        switch (this.sortBy) {
          case 'low':      return [...pkgs].sort((a, b) => a.price - b.price);
          case 'high':     return [...pkgs].sort((a, b) => b.price - a.price);
          case 'rating':   return [...pkgs].sort((a, b) => b.rating - a.rating);
          case 'duration': return [...pkgs].sort((a, b) => b.nights - a.nights);
          case 'discount': return [...pkgs].sort((a, b) => (b.discount || 0) - (a.discount || 0));
          default:         return pkgs;
        }
      })
    );
}
onSortChange() {
  this.loadPackages();
}
}