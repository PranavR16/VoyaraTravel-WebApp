import {
  Component,
  ViewEncapsulation,
  OnInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../core/services/toast';

interface SearchTab {
  id: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.scss',
  encapsulation: ViewEncapsulation.None
})
export class SearchBar implements OnInit {

  activeTab = 'flights';

  tabs: SearchTab[] = [
    { id: 'flights',    icon: '✈',  label: 'Flights'    },
    { id: 'hotels',     icon: '🏨',  label: 'Hotels'     },
    { id: 'packages',   icon: '🎒',  label: 'Packages'   },
    { id: 'cruises',    icon: '🚢',  label: 'Cruises'    },
    { id: 'activities', icon: '🎯',  label: 'Activities' },
  ];

  form = {
    from:      '',
    to:        '',
    departure: '',
    returnD:   '',
    travelers: '2 Adults',
    checkIn:   '',
    checkOut:  '',
    nights:    '7 Nights',
    budget:    '',
  };

  travelerOptions = [
    '1 Adult', '2 Adults',
    '2 Adults, 1 Child',
    '2 Adults, 2 Children',
    'Group (5+)'
  ];

  popularDestinations = [
    'Bali 🇮🇩', 'Paris 🇫🇷',
    'Dubai 🇦🇪', 'Tokyo 🇯🇵',
    'Maldives 🇲🇻', 'Switzerland 🇨🇭'
  ];

  constructor(
    private router: Router,
    private toast: ToastService
  ) {}

  ngOnInit() {}

  setTab(id: string) {
    this.activeTab = id;
  }

  setDestination(dest: string) {
    // Strip emoji from destination
    this.form.to = dest.split(' ')[0];
  }

  search() {
    if (!this.form.to && !this.form.from) {
      this.toast.error('Please enter a destination');
      return;
    }
    this.toast.info(`🔍 Searching for ${this.form.to || this.form.from}...`);
    this.router.navigate(['/search'], {
      queryParams: {
        to: this.form.to,
        from: this.form.from,
        tab: this.activeTab
      }
    });
  }
}