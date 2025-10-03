import { Injectable, inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { SubscriptionService } from '../services/subscription.service';

/**
 * Guard to check if user has an active subscription.
 * Redirects to /subscription/select if no subscription found.
 */
export const subscriptionGuard: CanActivateFn = async () => {
  const subscriptionService = inject(SubscriptionService);
  const router = inject(Router);

  console.log('[SubscriptionGuard] Checking subscription...');

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

    console.log('[SubscriptionGuard] Subscription active, allowing access');
    return true;
  } catch (error) {
    console.error('[SubscriptionGuard] Error checking subscription:', error);
    // On error, redirect to selection page to be safe
    return router.createUrlTree(['/subscription/select']);
  }
};
