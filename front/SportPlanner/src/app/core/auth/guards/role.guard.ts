import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthStateService } from '../services/auth-state.service';
import { UserRole } from '../models/user.model';

/**
 * Functional role-based authorization guard.
 * Checks if authenticated user has required role(s).
 * 
 * Usage in routes with data:
 * ```typescript
 * {
 *   path: 'admin',
 *   loadComponent: () => import('./admin.component'),
 *   canActivate: [authGuard, roleGuard],
 *   data: { roles: ['admin'] }
 * }
 * ```
 * 
 * @see https://angular.dev/guide/routing/common-router-tasks#preventing-unauthorized-access
 */
export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot
) => {
  const authStateService = inject(AuthStateService);
  const router = inject(Router);

  // Get required roles from route data
  const requiredRoles = route.data['roles'] as UserRole[] | undefined;
  
  if (!requiredRoles || requiredRoles.length === 0) {
    console.warn('Role guard: No roles specified in route data');
    return true; // No roles required, allow access
  }

  const user = authStateService.user();
  
  // User must be authenticated (should be checked by authGuard first)
  if (!user) {
    console.warn('Role guard: User not authenticated');
    return router.createUrlTree(['/auth/login']);
  }

  // Check if user has any of the required roles
  const hasRequiredRole = requiredRoles.includes(user.role);
  
  if (!hasRequiredRole) {
    console.warn(`Role guard: User lacks required role. Has: ${user.role}, Needs: ${requiredRoles.join(', ')}`);
    return router.createUrlTree(['/unauthorized']);
  }

  return true;
};
