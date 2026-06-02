import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, combineLatest, map } from 'rxjs';
import { TravelDataService, Package } from '../../core/services/travel-data';
import { BookingService } from '../../core/services/booking';
import { PackageCard } from '../../components/package-card/package-card';
import { debounceTime } from 'rxjs/operators';
import { ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule,PackageCard],
  templateUrl: './search.html',
  styleUrl: './search.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SearchComponent implements OnInit {

  // Use subjects to drive reactive filtering
  private sortBy$    = new BehaviorSubject<string>('popular');
  private maxBudget$ = new BehaviorSubject<number>(300000);
  private query$     = new BehaviorSubject<string>('');

  // UI-bound values
  sortBy    = 'popular';
  maxBudget = 300000;
  searchQuery = '';

  // Reactive filtered list
  filteredPackages$!: Observable<Package[]>;
  packages$!: Observable<Package[]>;

  constructor(
    public dataService: TravelDataService,
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnInit() {
    window.scrollTo({ top: 0, behavior: 'instant' });
    this.packages$ = this.dataService.getPackages();
    // Combine all filters reactively
    this.filteredPackages$ = combineLatest([
     // this.dataService.getPackages(),   // Observable<Package[]>
      this.packages$,
      this.sortBy$,
      this.maxBudget$,
      this.query$.pipe(debounceTime(300))
    ]).pipe(
      map(([pkgs, sort, budget, query]) => {

        // Filter
        let result = pkgs.filter(p => p.price <= budget);
        if (query.trim()) {
          const q = query.toLowerCase();
          result = result.filter(p =>
            p.name.toLowerCase().includes(q) ||
            p.category.toLowerCase().includes(q)
          );
        }

        // Sort
        switch (sort) {
          case 'low':      return [...result].sort((a, b) => a.price - b.price);
          case 'high':     return [...result].sort((a, b) => b.price - a.price);
          case 'rating':   return [...result].sort((a, b) => b.rating - a.rating);
          case 'duration': return [...result].sort((a, b) => b.nights - a.nights);
          case 'discount': return [...result].sort((a, b) => (b.discount || 0) - (a.discount || 0));
          default:         return result;
        }
      })
    );
  }

  onSortChange()   { this.sortBy$.next(this.sortBy); }
  onBudgetChange() { this.maxBudget$.next(this.maxBudget); }
  onQueryChange()  { this.query$.next(this.searchQuery); }

  onBookPackage(pkg: Package) {
    this.bookingService.selectedPackage.set(pkg);
    this.router.navigate(['/booking']);
  }

  clearFilters() {
    this.sortBy = 'popular';     this.sortBy$.next('popular');
    this.maxBudget = 300000;     this.maxBudget$.next(300000);
    this.searchQuery = '';       this.query$.next('');
  }
}

// import { Component } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { TravelDataService, Package } from '../../core/services/travel-data';
// import { BookingService } from '../../core/services/booking';
// import { PackageCard } from '../../components/package-card/package-card';
// import { Router } from '@angular/router';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-search',
//   standalone: true,
//   imports: [CommonModule, FormsModule],
//   templateUrl: './search.html',
//   styleUrl: './search.scss'
// })
// export class Search {
//   maxBudget = 200000;
//   sortBy = 'popular';
//   selectedCategories: string[] = [];

//   constructor(
//     public dataService: TravelDataService,
//     private bookingService: BookingService,
//     private router: Router
//   ) {}

//   get filteredPackages(): Package[] {
//     let pkgs = [...this.dataService.getPackagesByCategory('all')];
//     pkgs = pkgs.filter(p => p.price <= this.maxBudget);
//     if (this.sortBy === 'low') pkgs.sort((a, b) => a.price - b.price);
//     else if (this.sortBy === 'high') pkgs.sort((a, b) => b.price - a.price);
//     else if (this.sortBy === 'rating') pkgs.sort((a, b) => b.rating - a.rating);
//     return pkgs;
//   }

//   onBookPackage(pkg: Package) {
//     this.bookingService.selectedPackage.set(pkg);
//     this.router.navigate(['/booking']);
//   }
// }