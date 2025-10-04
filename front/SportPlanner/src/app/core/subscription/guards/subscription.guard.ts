import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { SubscriptionService } from '../services/subscription.service';
import { isPlatformBrowser } from '@angular/common';

/**
 * Guard to check if user has an active subscription.
 * Redirects to /subscription/select if no subscription found.
 */
export const subscriptionGuard: CanActivateFn = async () => {
  const subscriptionService = inject(SubscriptionService);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  // Avoid running the guard's HTTP checks during server-side rendering
  if (!isPlatformBrowser(platformId)) {
    // Minimal log to make CI/debugging clearer, but skip network calls
    console.log('[SubscriptionGuard] Skipping subscription check on server');
    return true; // allow navigation during SSR; client will re-check
  }

  try {
    const subscription = await subscriptionService.getMySubscription();

    if (!subscription) {
      console.log('[SubscriptionGuard] No subscription found, redirecting to selection page');
      return router.createUrlTree(['/subscription/select']);
    }

    if (!subscription.isActive) {
      console.log('[SubscriptionGuard] Subscription is inactive, redirecting to selection page');
      return router.createUrlTree(['/subscription/select']);
    }

    return true;
  } catch (error) {
    console.error('[SubscriptionGuard] Error checking subscription:', error);
    // On error, redirect to selection page to be safe
    return router.createUrlTree(['/subscription/select']);
  }
};
