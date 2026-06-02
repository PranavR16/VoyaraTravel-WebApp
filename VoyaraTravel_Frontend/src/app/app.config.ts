// import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
// import { provideRouter, withInMemoryScrolling } from '@angular/router';
// import { routes } from './app.routes';

// export const appConfig: ApplicationConfig = {
//   providers: [
//     provideBrowserGlobalErrorListeners(),
//     provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'top' }))
//   ]
// };

// src/app/app.config.ts
// UPDATED version — adds HttpClient + auth interceptor
import { ApplicationConfig, } from '@angular/core';
import { PreloadAllModules, provideRouter ,withInMemoryScrolling,withPreloading } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withInMemoryScrolling (),withPreloading(PreloadAllModules)),
    provideAnimations(),

    // ✅ Add this — enables HTTP calls + attaches JWT automatically
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    
  ]
};
