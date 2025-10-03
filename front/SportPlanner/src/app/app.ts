import { Component, signal, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { Navbar } from './features/shared/components/navbar/navbar';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';
import { LanguageService } from './services/language.service';
import { AuthStateService } from './core/auth/services/auth-state.service';
import { TokenService } from './core/auth/services/token.service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('SportPlanner');
  showNavbar = signal(true);

  // Services
  private languageService = inject(LanguageService);
  private authStateService = inject(AuthStateService);
  private tokenService = inject(TokenService);
  private router = inject(Router);

  constructor() {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        // Ocultar navbar en las rutas /auth y /dashboard
        this.showNavbar.set(!event.url.includes('/auth') && !event.url.includes('/dashboard'));
      });
  }

  async ngOnInit() {
    // Auto-login: Check if user has valid token on app initialization
    await this.checkAuthenticationStatus();
  }

  private async checkAuthenticationStatus() {
    // Wait for auth state to initialize
    await this.authStateService.whenReady(2000);

    // If user is already authenticated from token in localStorage, we're good
    if (this.authStateService.isAuthenticated()) {
      console.log('App: User already authenticated from stored token');
      return;
    }

    // If not authenticated, try to refresh token using HttpOnly cookie
    console.log('App: No valid token found, attempting token refresh...');
    const refreshed = await this.attemptTokenRefresh();

    if (refreshed) {
      console.log('App: Token refresh successful, user auto-logged in');
    } else {
      console.log('App: Token refresh failed or no refresh token available');
    }
  }

  private async attemptTokenRefresh(): Promise<boolean> {
    // Skip auto-refresh during SSR (server-side rendering)
    if (typeof window === 'undefined') {
      return false;
    }

    try {
      const response = await fetch(`${environment.apiBaseUrl}/api/auth/refresh`, {
        method: 'POST',
        credentials: 'include', // Send HttpOnly cookies
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        console.log('App: Token refresh failed with status:', response.status);
        return false;
      }

      const data = await response.json();

      if (data?.accessToken) {
        // Store new access token
        this.tokenService.setTokens(data.accessToken);

        // Refresh auth state to decode and set user
        this.authStateService.refreshAuthState();

        // Wait for state to update
        await new Promise(resolve => setTimeout(resolve, 100));

        return this.authStateService.isAuthenticated();
      }

      return false;
    } catch (error) {
      // Silently fail if refresh is not available (expected on first visit)
      // Only log if it's not a network/SSL error
      if (error instanceof Error && !error.message.includes('fetch failed')) {
        console.error('App: Token refresh error:', error);
      }
      return false;
    }
  }
}
