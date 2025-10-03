import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * Service for secure JWT token storage.
 * Uses localStorage only in browser environment (SSR-safe).
 * 
 * @see .clinerules/13-supabase-jwt.md for security considerations
 */
@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly TOKEN_KEY = 'sportplanner-access-token';
  private readonly REFRESH_TOKEN_KEY = 'sportplanner-refresh-token';
  private platformId = inject(PLATFORM_ID);

  /**
   * Check if running in browser (not SSR).
   */
  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  /**
   * Store access and refresh tokens.
   */
  setTokens(accessToken: string, refreshToken?: string): void {
    if (!this.isBrowser()) return;

    localStorage.setItem(this.TOKEN_KEY, accessToken);
    if (refreshToken) {
      localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    }
  }

  /**
   * Retrieve access token.
   */
  getAccessToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Retrieve refresh token.
   */
  getRefreshToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Clear all stored tokens.
   */
  clearTokens(): void {
    if (!this.isBrowser()) return;
    
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Check if access token exists.
   */
  hasToken(): boolean {
    return !!this.getAccessToken();
  }
}
