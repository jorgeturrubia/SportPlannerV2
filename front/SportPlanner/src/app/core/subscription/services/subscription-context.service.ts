import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface Subscription {
  id: string;
  ownerId: string;
  type: string;
  sport: string;
  maxUsers: number;
  maxTeams: number;
  isActive: boolean;
  createdAt: string;
}

/**
 * Service to manage subscription context for the current user.
 * Fetches and caches the user's subscription information.
 */
@Injectable({
  providedIn: 'root'
})
export class SubscriptionContextService {
  private http = inject(HttpClient);
  private apiBaseUrl = environment.apiBaseUrl;

  // Signals for subscription state
  private subscriptionSignal = signal<Subscription | null>(null);
  private isLoadingSignal = signal<boolean>(false);
  private errorSignal = signal<string | null>(null);

  // Public readonly signals
  readonly subscription = this.subscriptionSignal.asReadonly();
  readonly isLoading = this.isLoadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly subscriptionId = computed(() => this.subscriptionSignal()?.id ?? null);
  readonly hasSubscription = computed(() => this.subscriptionSignal() !== null);

  /**
   * Fetch the current user's subscription from the backend.
   * Caches the result in a signal.
   */
  async loadSubscription(): Promise<Subscription | null> {
    if (this.isLoadingSignal()) {
      // Already loading, return current value
      return this.subscriptionSignal();
    }

    this.isLoadingSignal.set(true);
    this.errorSignal.set(null);

    try {
      const subscription = await firstValueFrom(
        this.http.get<Subscription>(`${this.apiBaseUrl}/api/subscription/my`)
      );

      this.subscriptionSignal.set(subscription);
      return subscription;
    } catch (error: any) {
      console.error('Failed to load subscription:', error);

      if (error.status === 404) {
        this.errorSignal.set('No subscription found. Please create one.');
      } else {
        this.errorSignal.set('Failed to load subscription information.');
      }

      this.subscriptionSignal.set(null);
      return null;
    } finally {
      this.isLoadingSignal.set(false);
    }
  }

  /**
   * Get subscription ID synchronously.
   * Returns null if not loaded yet.
   */
  getSubscriptionId(): string | null {
    return this.subscriptionId();
  }

  /**
   * Get subscription ID, loading it first if necessary.
   * Throws error if subscription cannot be loaded.
   */
  async ensureSubscriptionId(): Promise<string> {
    // If already loaded, return immediately
    const currentId = this.subscriptionId();
    if (currentId) {
      return currentId;
    }

    // Load subscription
    const subscription = await this.loadSubscription();

    if (!subscription?.id) {
      throw new Error('No subscription available for this user');
    }

    return subscription.id;
  }

  /**
   * Clear cached subscription data.
   */
  clear(): void {
    this.subscriptionSignal.set(null);
    this.errorSignal.set(null);
    this.isLoadingSignal.set(false);
  }
}
