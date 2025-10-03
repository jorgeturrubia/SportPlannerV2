import { inject } from '@angular/core';
import { Router, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { AuthStateService } from '../services/auth-state.service';

/**
 * Functional auth guard for protecting routes.
 * Redirects to login if user is not authenticated.
 * 
 * Usage in routes:
 * ```typescript
 * {
 *   path: 'dashboard',
 *   loadComponent: () => import('./dashboard.component'),
 *   canActivate: [authGuard]
 * }
 * ```
 * 
 * @see https://angular.dev/guide/routing/common-router-tasks#preventing-unauthorized-access
 * @see .clinerules/02-angular-structure.md for routing conventions
 */
export const authGuard: CanActivateFn = async (
  _route,
  state: RouterStateSnapshot
) => {
  const authStateService = inject(AuthStateService);
  const router = inject(Router);

  // Wait for auth initialization (with timeout)
  const ready = await authStateService.whenReady(2000);

  if (!ready && authStateService.isLoading()) {
    // Initialization didn't finish in time but still loading: deny access
    console.warn('Auth guard: auth state not ready in time');
    return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
  }

  if (authStateService.isAuthenticated()) {
    return true;
  }

  // Not authenticated - redirect to login with return URL
  return router.createUrlTree(['/auth/login'], {
    queryParams: { returnUrl: state.url }
  });
};
