import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { TokenService } from './token.service';
import { AuthStateService } from './auth-state.service';
import { User, LoginCredentials, RegisterPayload, ApiAuthResponse, mapApiResponseToUser } from '../models/user.model';
import { environment } from '../../../../environments/environment';

/**
 * Core authentication service calling .NET backend API.
 * Backend handles Supabase Auth integration.
 * 
 * @see back/SportPlanner/src/SportPlanner.API/Controllers/AuthController.cs
 * @see .clinerules/13-supabase-jwt.md
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private platformId = inject(PLATFORM_ID);
  private http = inject(HttpClient);
  private tokenService = inject(TokenService);
  private authStateService = inject(AuthStateService);
  private router = inject(Router);
  private apiBaseUrl = environment.apiBaseUrl;

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  /**
   * Sign in with email and password via backend API.
   * Stores JWT token and returns user on success.
   */
  async signIn(credentials: LoginCredentials): Promise<User> {
    if (!this.isBrowser()) {
      throw new Error('Authentication is only available in browser');
    }

    try {
      const response = await firstValueFrom(
        this.http.post<ApiAuthResponse>(`${this.apiBaseUrl}/api/auth/login`, {
          email: credentials.email,
          password: credentials.password
        })
      );

      // Store JWT token
      this.tokenService.setTokens(response.accessToken);
      console.log('[AuthService] Token stored in localStorage:', !!response.accessToken);

      // Map API response to User model
      const user = mapApiResponseToUser(response);

      // CRITICAL: Update auth state service immediately to sync accessTokenSignal
      // This ensures the interceptor has access to the token for subsequent requests
      this.authStateService.setAuthenticatedUser(user, response.accessToken);
      console.log('[AuthService] Auth state updated, accessToken signal set:', !!this.authStateService.accessToken());

      return user;
    } catch (error) {
      throw this.mapHttpError(error);
    }
  }

  /**
   * Register new user via backend API.
   */
  async signUp(payload: RegisterPayload): Promise<User> {
    if (!this.isBrowser()) {
      throw new Error('Authentication is only available in browser');
    }

    try {
      // Split displayName into firstName and lastName
      const nameParts = (payload.displayName || payload.email.split('@')[0]).split(' ');
      const firstName = nameParts[0] || 'User';
      const lastName = nameParts.slice(1).join(' ') || '';

      const response = await firstValueFrom(
        this.http.post<ApiAuthResponse>(`${this.apiBaseUrl}/api/auth/register`, {
          firstName,
          lastName,
          email: payload.email,
          password: payload.password
        })
      );

      // Map API response to User model
      const user = mapApiResponseToUser(response);

      // Store JWT token if present
      if (response.accessToken) {
        this.tokenService.setTokens(response.accessToken);

        // CRITICAL: Update auth state service immediately to sync accessTokenSignal
        this.authStateService.setAuthenticatedUser(user, response.accessToken);
      }

      return user;
    } catch (error) {
      throw this.mapHttpError(error);
    }
  }

  /**
   * Sign out current user.
   * Clears tokens and redirects to login.
   */
  async signOut(): Promise<void> {
    if (!this.isBrowser()) return;

    try {
      // Ask backend to clear refresh cookie (backend will delete HttpOnly cookie)
      await firstValueFrom(this.http.post(`${this.apiBaseUrl}/api/auth/logout`, {}));
    } catch {
      // ignore errors
    }

    // Clear local tokens and state
    this.tokenService.clearTokens();
    this.authStateService.clearAuthState();
    this.router.navigate(['/auth']);
  }

  /**
   * Get current authenticated user from token.
   * Note: We don't have session info anymore, just the JWT token.
   */
  async getCurrentUser(): Promise<User | null> {
    if (!this.isBrowser()) return null;

    const token = this.tokenService.getAccessToken();
    if (!token) return null;

    // TODO: Optionally decode JWT to get user info
    // For now, return null and rely on AuthStateService initialization
    return null;
  }

  /**
   * Map HTTP errors to user-friendly messages.
   * Now extracts specific Supabase error messages from backend.
   */
  private mapHttpError(error: unknown): Error {
    if (error instanceof HttpErrorResponse) {
      // Backend now returns { error: "mensaje específico de Supabase" }
      if (error.error?.error) {
        return new Error(error.error.error);
      }

      // Fallback a mensajes genéricos por status code
      const errorMessages: Record<number, string> = {
        400: 'Datos inválidos. Verifica la información ingresada.',
        401: 'Email o contraseña incorrectos',
        409: 'Este email ya está registrado',
        429: 'Demasiados intentos. Espera unos minutos.',
        500: 'Error del servidor. Intenta nuevamente.'
      };

      const message = errorMessages[error.status] || 'Error al autenticar';
      return new Error(message);
    }

    return error instanceof Error ? error : new Error('Error desconocido');
  }
}
