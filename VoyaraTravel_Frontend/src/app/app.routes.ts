import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () => import('./feature/home/home').then((m) => m.Home),
  },
  {
    path: 'login',
    loadComponent: () => import('./feature/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'profile',
    redirectTo: 'login', // login page handles profile tab
    pathMatch: 'full',
  },
  {
    path: 'packages',
    loadComponent: () => import('./feature/packages/packages').then((m) => m.Packages),
  },
  {
    path: 'search',
    loadComponent: () => import('./feature/search/search').then((m) => m.SearchComponent),
  },
  {
    path: 'booking',
    loadComponent: () => import('./feature/booking/booking').then((m) => m.Booking),
  },
  {
    path: 'payment',
    loadComponent: () => import('./feature/payment/payment').then((m) => m.Payment),
  },
  {
  path: 'offers',
  loadComponent: () => import('./feature/offers/offers').then(m => m.Offers)
},
  { path: '**', redirectTo: 'home' },
];