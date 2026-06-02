import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private counter = 0;
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.toastsSubject.asObservable();

  show(
    message: string,
    type: 'success' | 'error' | 'info' = 'success',
    duration = 3500
  ) {
    const id = ++this.counter;
    const current = this.toastsSubject.getValue();
    this.toastsSubject.next([...current, { id, message, type }]);
    setTimeout(() => this.remove(id), duration);
  }

  remove(id: number) {
    const current = this.toastsSubject.getValue();
    this.toastsSubject.next(current.filter(t => t.id !== id));
  }

  success(msg: string) { this.show(msg, 'success'); }
  error(msg: string)   { this.show(msg, 'error'); }
  info(msg: string)    { this.show(msg, 'info'); }
}


// import { Injectable } from '@angular/core';
// import { BehaviorSubject } from 'rxjs';

// export interface Toast {
//   id: number;
//   message: string;
//   type: 'success' | 'error' | 'info';
// }

// @Injectable({ providedIn: 'root' })
// export class ToastService {
//   private counter = 0;
//   private toastsSubject = new BehaviorSubject<Toast[]>([]);
//   toasts$ = this.toastsSubject.asObservable();

//   show(
//     message: string,
//     type: 'success' | 'error' | 'info' = 'success',
//     duration = 3500
//   ) {
//     const id = ++this.counter;
//     const current = this.toastsSubject.getValue();
//     this.toastsSubject.next([...current, { id, message, type }]);
//     setTimeout(() => this.remove(id), duration);
//   }

//   remove(id: number) {
//     const current = this.toastsSubject.getValue();
//     this.toastsSubject.next(current.filter(t => t.id !== id));
//   }

//   // Shorthand methods
//   success(msg: string) { this.show(msg, 'success'); }
//   error(msg: string)   { this.show(msg, 'error'); }
//   info(msg: string)    { this.show(msg, 'info'); }
// }

// import { Injectable } from '@angular/core';
// import { BehaviorSubject } from 'rxjs';

// export interface Toast {
//   id: number;
//   message: string;
//   type: 'success' | 'error' | 'info';
// }

// @Injectable({ providedIn: 'root' })
// export class ToastService {
//   private counter = 0;
//   private toastsSubject = new BehaviorSubject<Toast[]>([]);
//   toasts$ = this.toastsSubject.asObservable();

//   show(message: string, type: 'success' | 'error' | 'info' = 'success', duration = 3500) {
//     const id = ++this.counter;
//     const current = this.toastsSubject.getValue();
//     this.toastsSubject.next([...current, { id, message, type }]);
//     setTimeout(() => this.remove(id), duration);
//   }

//   remove(id: number) {
//     const current = this.toastsSubject.getValue();
//     this.toastsSubject.next(current.filter(t => t.id !== id));
//   }

//   success(msg: string) { this.show(msg, 'success'); }
//   error(msg: string)   { this.show(msg, 'error'); }
//   info(msg: string)    { this.show(msg, 'info'); }
// }