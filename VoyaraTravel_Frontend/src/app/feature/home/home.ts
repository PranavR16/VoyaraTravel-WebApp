import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Destination, TravelDataService } from '../../core/services/travel-data';
import { BookingService } from '../../core/services/booking';
import { PackageCard } from '../../components/package-card/package-card';
import { SearchBar } from '../../shared/search-bar/search-bar';
import { CountdownTimer} from '../../shared/countdown-timer/countdown-timer';
import { FormsModule } from '@angular/forms';
import { map, Observable } from 'rxjs';
import { ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, PackageCard, SearchBar, CountdownTimer,FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Home implements OnInit, OnDestroy {
  activeFilter = 'all';
  private currentFilter = 'all';
  nlEmail = '';
  //private particleInterval: any;
  //filters: any;
  destinations$ !: Observable<Destination[]>;
  filteredPackages$!: Observable<any[]>;
  filters = [
  { label: 'All', value: 'all' },
  { label: 'Luxury', value: 'luxury' },
  { label: 'Adventure', value: 'adventure' }
];
  
  constructor(
    public dataService: TravelDataService,
    private bookingService: BookingService,
    public router: Router
  ) {}

  // get filteredPackages() {
  //   return this.dataService.getPackagesByCategory(this.activeFilter).slice(0, 4);
  // }
//   get filteredPackages() {
//   return this.dataService.getPackagesByCategory(this.activeFilter).pipe(
//     map(pkgs => pkgs.slice(0, 4))
//   );

// }
loadPackages() {
  this.filteredPackages$ = this.dataService
    .getPackagesByCategory(this.activeFilter)
    .pipe(map(pkgs => pkgs.slice(0, 4)));
}


  ngOnInit() {
    //this.createParticles();
    setTimeout(() => this.createParticles(), 0);
    // this.destinations$ = this.dataService.getDestinations();
    // this.loadPackages();
    setTimeout(() => {
      this.destinations$ = this.dataService.getDestinations();
    }, 0);

    setTimeout(() => this.loadPackages(), 0);
    setTimeout(() => this.createParticles(), 0);
  }

  ngOnDestroy() {
    //clearInterval(this.particleInterval);
  }

  // setFilter(cat: string) {
  //   this.activeFilter = cat;
  //   this.loadPackages(); //
  // }
  setFilter(cat: string) {
  if (this.currentFilter === cat) return; // avoid duplicate calls

  this.currentFilter = cat;
  this.activeFilter = cat;
  this.loadPackages();
}

  onBookPackage(pkg: any) {
    this.bookingService.selectedPackage.set(pkg);
    this.router.navigate(['/booking']);
  }

  subscribeNewsletter() {
    if (this.nlEmail && this.nlEmail.includes('@')) {
      alert(`🎉 Subscribed! Deals coming to ${this.nlEmail}`);
      this.nlEmail = '';
    }
  }

  createParticles() {
    const container = document.getElementById('heroParticles');
    if (!container) return;
    for (let i = 0; i < 20; i++) {
      const p = document.createElement('div');
      p.className = 'ptcl';
      const s = 1 + Math.random() * 4;
      p.style.cssText = `
        left:${Math.random() * 100}%;
        width:${s}px; height:${s}px;
        animation-duration:${9 + Math.random() * 14}s;
        animation-delay:${Math.random() * 10}s;
      `;
      container.appendChild(p);
    }
  }
}

