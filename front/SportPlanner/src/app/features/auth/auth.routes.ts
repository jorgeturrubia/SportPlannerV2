import { Routes } from '@angular/router';

/**
 * Auth feature routes.
 * All routes are lazy-loaded using loadComponent.
 * 
 * @see .clinerules/02-angular-structure.md for routing conventions
 */
export const authRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/auth-page/auth-page').then(m => m.AuthPage),
    title: 'Autenticación - SportPlanner'
  }
];
