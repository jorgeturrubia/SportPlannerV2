import { Injectable, signal, computed, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { TokenService } from './token.service';
import { User, AuthState } from '../models/user.model';

/**
 * Global authentication state using Angular Signals.
 * Manages user state based on JWT token from backend.
 * 
 * @see https://angular.dev/guide/signals
 * @see back/SportPlanner/src/SportPlanner.API/Controllers/AuthController.cs
 */
@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private platformId = inject(PLATFORM_ID);
  private tokenService = inject(TokenService);

  // Signal-based state
  private userSignal = signal<User | null>(null);
  private isLoadingSignal = signal<boolean>(true);
  private accessTokenSignal = signal<string | null>(null);

  // Public computed signals
  readonly user = this.userSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.userSignal() !== null);
  readonly isLoading = this.isLoadingSignal.asReadonly();
  readonly accessToken = this.accessTokenSignal.asReadonly();

  // Full auth state
  readonly authState = computed<AuthState>(() => ({
    user: this.userSignal(),
    isAuthenticated: this.isAuthenticated(),
    isLoading: this.isLoadingSignal(),
    accessToken: this.accessTokenSignal(),
  }));

  constructor() {
    // Initialize auth state on browser only
    if (isPlatformBrowser(this.platformId)) {
      this.initializeAuthState();
    } else {
      this.isLoadingSignal.set(false);
    }
  }

  // Promise resolver used to signal when initial auth load finished
  private readyResolver: (() => void) | null = null;
  private readyPromise: Promise<void> | null = null;

  /**
   * Returns a promise that resolves when auth initialization finishes or
   * when the optional timeout elapses. Resolves to true if initialized,
   * false if timed out.
   */
  whenReady(timeoutMs = 2000): Promise<boolean> {
    // If already finished, resolve immediately
    if (!this.isLoadingSignal()) return Promise.resolve(true);

    // Create promise once
    if (!this.readyPromise) {
      this.readyPromise = new Promise<void>((resolve) => {
        this.readyResolver = () => resolve();
      });
    }

    // Race between initialization and timeout
    return Promise.race([
      this.readyPromise.then(() => true),
      new Promise<boolean>((res) => setTimeout(() => res(false), timeoutMs)),
    ]);
  }

  /**
   * Initialize auth state from stored JWT token.
   */
  private initializeAuthState(): void {
    try {
      const token = this.tokenService.getAccessToken();

      if (token) {
        // Token exists - user is authenticated
        this.accessTokenSignal.set(token);

        // Decode JWT to get user info (optional - for now set minimal user)
        // In a real scenario, you might want to call backend to verify token
        // or decode JWT client-side to extract user claims
        const decodedUser = this.decodeTokenToUser(token);
        this.userSignal.set(decodedUser);
      } else {
        // No token - user is not authenticated
        this.userSignal.set(null);
        this.accessTokenSignal.set(null);
      }
    } catch (error) {
      console.error('Failed to initialize auth state:', error);
      this.userSignal.set(null);
      this.accessTokenSignal.set(null);
    } finally {
      this.isLoadingSignal.set(false);
      // Resolve when initial load finishes
      if (this.readyResolver) {
        try { this.readyResolver(); } catch { /* ignore */ }
      }
    }
  }

  /**
   * Decode JWT token to extract basic user info.
   * For production, consider using a proper JWT library.
   */
  private decodeTokenToUser(token: string): User | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      const user: User = {
        id: payload.sub || payload.userId || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '',
        emailConfirmed: true,
        role: (payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'admin') as any,
        createdAt: new Date(),
        lastSignIn: new Date(),
        displayName: payload.given_name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname'] || payload.email?.split('@')[0] || 'User',
        avatarUrl: undefined
      };

      return user;
    } catch (error) {
      console.error('AuthStateService: Failed to decode token', error);
      return null;
    }
  }

  /**
   * Manually refresh auth state.
   * Useful after external auth changes (e.g., after login).
   */
  refreshAuthState(): void {
    this.isLoadingSignal.set(true);
    this.initializeAuthState();
  }

  /**
   * Update user state after successful login/register.
   * Called by auth components after backend authentication.
   */
  setAuthenticatedUser(user: User, token: string): void {
    this.userSignal.set(user);
    this.accessTokenSignal.set(token);
    this.isLoadingSignal.set(false);
  }

  /**
   * Clear auth state (called on logout).
   */
  clearAuthState(): void {
    this.userSignal.set(null);
    this.accessTokenSignal.set(null);
    this.isLoadingSignal.set(false);
  }
}
