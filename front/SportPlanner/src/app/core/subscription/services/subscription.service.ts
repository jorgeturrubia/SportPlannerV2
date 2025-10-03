import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TokenService } from '../../auth/services/token.service';

export type SubscriptionType = 'Free' | 'Club' | 'Team' | 'Coach';
export type Sport = 'Football' | 'Basketball' | 'Handball';

export interface CreateSubscriptionRequest {
  type: SubscriptionType;
  sport: Sport;
}

export interface Subscription {
  id: string;
  ownerId: string;
  type: SubscriptionType;
  sport: Sport;
  maxUsers: number;
  maxTeams: number;
  isActive: boolean;
  createdAt: string;
}

/**
 * Service for managing user subscriptions.
 * Handles creation and retrieval of subscription data.
 */
@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private http = inject(HttpClient);
  private tokenService = inject(TokenService);
  private apiBaseUrl = environment.apiBaseUrl;

  /**
   * Create a new subscription for the current user.
   * @throws Error if subscription already exists or creation fails
   */
  async createSubscription(request: CreateSubscriptionRequest): Promise<string> {
    try {
      // Debug: Check if we have a token
      const token = this.tokenService.getAccessToken();
      console.log('[SubscriptionService] Token available:', !!token);
      if (token) {
        console.log('[SubscriptionService] Token preview:', token.substring(0, 50) + '...');
      } else {
        console.error('[SubscriptionService] NO TOKEN FOUND! User may not be properly authenticated.');
      }
      
      const subscriptionId = await firstValueFrom(
        this.http.post<string>(`${this.apiBaseUrl}/api/subscription`, request)
      );

      return subscriptionId;
    } catch (error: any) {
      console.error('[SubscriptionService] Failed to create subscription:', error);
      console.error('[SubscriptionService] Error status:', error.status);
      console.error('[SubscriptionService] Error message:', error.message);
      throw new Error(error.error?.error || 'No se pudo crear la suscripción');
    }
  }

  /**
   * Get the current user's subscription.
   * @returns Subscription object or null if not found
   */
  async getMySubscription(): Promise<Subscription | null> {
    try {
      const subscription = await firstValueFrom(
        this.http.get<Subscription>(`${this.apiBaseUrl}/api/subscription/my`)
      );

      return subscription;
    } catch (error: any) {
      if (error.status === 404) {
        return null; // User has no subscription
      }

      console.error('Failed to get subscription:', error);
      throw error;
    }
  }
}
