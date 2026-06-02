import {
  Component,
  OnInit,
  OnDestroy,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
  ViewEncapsulation
} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-countdown-timer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './countdown-timer.html',
  styleUrl: './countdown-timer.scss',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush  // ← explicit
})
export class CountdownTimer implements OnInit, OnDestroy {

  days    = '02';
  hours   = '14';
  minutes = '30';
  seconds = '00';

  private interval: any;
  private targetDate!: Date;

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.targetDate = new Date();
    this.targetDate.setDate(this.targetDate.getDate() + 2);
    this.targetDate.setHours(this.targetDate.getHours() + 14);
    this.targetDate.setMinutes(this.targetDate.getMinutes() + 30);

    // Run once immediately
    this.tick();

    // Start interval
    this.interval = setInterval(() => {
      this.tick();
      this.cdr.markForCheck(); // ← tell Angular to re-render
    }, 1000);
  }

  ngOnDestroy() {
    if (this.interval) clearInterval(this.interval);
  }

  private tick() {
    const now  = new Date().getTime();
    const diff = this.targetDate.getTime() - now;

    if (diff <= 0) {
      this.days = this.hours = this.minutes = this.seconds = '00';
      clearInterval(this.interval);
      this.cdr.markForCheck();
      return;
    }

    const d = Math.floor(diff / (1000 * 60 * 60 * 24));
    const h = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const m = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const s = Math.floor((diff % (1000 * 60)) / 1000);

    this.days    = String(d).padStart(2, '0');
    this.hours   = String(h).padStart(2, '0');
    this.minutes = String(m).padStart(2, '0');
    this.seconds = String(s).padStart(2, '0');

    this.cdr.markForCheck(); // ← force view update
  }
}