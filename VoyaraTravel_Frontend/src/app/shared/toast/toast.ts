import {
  Component,
  OnDestroy,
  ViewEncapsulation
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../../core/services/toast';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div class="toast-container">
      <div
        class="toast"
        *ngFor="let toast of toasts"
        [class.success]="toast.type === 'success'"
        [class.error]="toast.type === 'error'"
        [class.info]="toast.type === 'info'">

        <span class="toast-icon">
          {{ toast.type === 'success' ? '✓' : toast.type === 'error' ? '✕' : 'ℹ' }}
        </span>

        <span class="toast-msg">{{ toast.message }}</span>

        <button class="toast-close" (click)="remove(toast.id)">×</button>

      </div>
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 28px;
      right: 28px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 340px;
      pointer-events: none;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: 12px;
      border-radius: 14px;
      padding: 14px 16px;
      box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
      min-width: 280px;
      border-left: 3px solid transparent;
      pointer-events: all;
      animation: toastSlideIn 0.35s cubic-bezier(0.4, 0, 0.2, 1) both;
    }

    @keyframes toastSlideIn {
      from {
        opacity: 0;
        transform: translateX(60px);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }

    .toast.success {
      background: rgba(14, 26, 20, 0.98);
      border: 1px solid rgba(76, 175, 132, 0.25);
      border-left-color: #4CAF84;
    }

    .toast.error {
      background: rgba(26, 12, 12, 0.98);
      border: 1px solid rgba(201, 76, 76, 0.25);
      border-left-color: #C94C4C;
    }

    .toast.info {
      background: rgba(10, 20, 32, 0.98);
      border: 1px solid rgba(76, 154, 201, 0.25);
      border-left-color: #4C9AC9;
    }

    .toast-icon {
      width: 30px;
      height: 30px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 13px;
      font-weight: 700;
      flex-shrink: 0;
    }

    .toast.success .toast-icon {
      background: rgba(76, 175, 132, 0.15);
      color: #4CAF84;
    }

    .toast.error .toast-icon {
      background: rgba(201, 76, 76, 0.15);
      color: #C94C4C;
    }

    .toast.info .toast-icon {
      background: rgba(76, 154, 201, 0.15);
      color: #4C9AC9;
    }

    .toast-msg {
      flex: 1;
      font-size: 13px;
      color: #EEEEF5;
      line-height: 1.5;
      font-family: 'DM Sans', sans-serif;
    }

    .toast-close {
      background: none;
      border: none;
      color: #666688;
      font-size: 20px;
      cursor: pointer;
      padding: 0 4px;
      line-height: 1;
      flex-shrink: 0;
      transition: color 0.2s;
      font-family: sans-serif;
    }

    .toast-close:hover {
      color: #EEEEF5;
    }

    @media (max-width: 480px) {
      .toast-container {
        bottom: 16px;
        right: 12px;
        left: 12px;
        max-width: 100%;
      }
      .toast {
        min-width: unset;
        width: 100%;
      }
    }
  `]
})
export class ToastComponent implements OnDestroy {
  toasts: Toast[] = [];
  private sub: Subscription;

  constructor(private toastService: ToastService) {
    this.sub = this.toastService.toasts$.subscribe(toasts => {
      this.toasts = toasts;
    });
  }

  remove(id: number) {
    this.toastService.remove(id);
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}


// import { Component, OnDestroy, ViewEncapsulation} from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { animation, style, animate, useAnimation } from '@angular/animations';
// import { ToastService, Toast } from '../../services/toast';
// import { Subscription } from 'rxjs';

// // Define reusable animation 
// const fadeIn = animation([ 
//   style({ opacity: 0 }), 
//   animate('{{ time }} ease-out', style({ opacity: 1 })) ], 
//   { params: { time: '200ms' } });

// @Component({
//   selector: 'app-toast',
//   standalone: true,
//   imports: [CommonModule],
//   templateUrl: './toast.html',
//   styleUrl: './toast.scss',
//   encapsulation: ViewEncapsulation.None,

//   // ✅ animations MUST be declared here inside @Component
//   animations: [
//     trigger('toastAnim', [
//       transition(':enter', [
//         style({ opacity: 0, transform: 'translateX(60px)' }),
//         animate(
//           '350ms cubic-bezier(0.4, 0, 0.2, 1)',
//           style({ opacity: 1, transform: 'translateX(0)' })
//         )
//       ]),
//       transition(':leave', [
//         animate(
//           '250ms cubic-bezier(0.4, 0, 0.2, 1)',
//           style({ opacity: 0, transform: 'translateX(60px)' })
//         )
//       ])
//     ])
//   ]
// })
// export class ToastComponent implements OnDestroy {
//   toasts: Toast[] = [];
//   private sub: Subscription;

//   constructor(private toastService: ToastService) {
//     this.sub = this.toastService.toasts$.subscribe(toasts => {
//       this.toasts = toasts;
//     });
//   }

//   remove(id: number) {
//     this.toastService.remove(id);
//   }

//   ngOnDestroy() {
//     this.sub.unsubscribe();
//   }
// }



// import {
//   Component,
//   OnDestroy
// } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { trigger, transition, style, animate } from '@angular/animations';
// import { ToastService, Toast } from '../../services/toast';
// import { Subscription } from 'rxjs';

// @Component({
//   selector: 'app-toast',
//   standalone: true,
//   imports: [CommonModule],
//   templateUrl: './toast.html',
//   styleUrl: './toast.scss',
//   animations: [
//     trigger('toastAnim', [
//       transition(':enter', [
//         style({ opacity: 0, transform: 'translateX(60px)' }),
//         animate('350ms cubic-bezier(0.4,0,0.2,1)',
//           style({ opacity: 1, transform: 'translateX(0)' }))
//       ]),
//       transition(':leave', [
//         animate('250ms cubic-bezier(0.4,0,0.2,1)',
//           style({ opacity: 0, transform: 'translateX(60px)' }))
//       ])
//     ])
//   ]
// })
// export class ToastComponent implements OnDestroy {
//   toasts: Toast[] = [];
//   private sub: Subscription;

//   constructor(private toastService: ToastService) {
//     this.sub = this.toastService.toasts$.subscribe(toasts => {
//       this.toasts = toasts;
//     });
//   }

//   remove(id: number) {
//     this.toastService.remove(id);
//   }

//   ngOnDestroy() {
//     this.sub.unsubscribe();
//   }
// }