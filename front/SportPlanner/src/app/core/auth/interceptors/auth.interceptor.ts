import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthStateService } from '../services/auth-state.service';
import { TokenService } from '../services/token.service';
import { environment } from '../../../../environments/environment';

// Simple refresh queue to avoid parallel refresh calls
let isRefreshing = false;
let refreshPromise: Promise<string | null> | null = null;

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const authStateService = inject(AuthStateService);
  const tokenService = inject(TokenService);

  // Skip auth for public routes
  if (shouldSkipAuth(req.url)) {
    return next(req);
  }

  const accessToken = authStateService.accessToken();

  const authReq = accessToken ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } }) : req;

  return next(authReq).pipe(
    catchError((err) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        // Try to refresh token
        if (!isRefreshing) {
          isRefreshing = true;
          refreshPromise = fetchNewAccessToken().then(t => {
            isRefreshing = false;
            return t;
          }).catch(() => {
            isRefreshing = false;
            return null;
          });
        }

        return from(refreshPromise!).pipe(
          switchMap((newToken) => {
            if (newToken) {
              tokenService.setTokens(newToken);

              // CRITICAL: Refresh auth state to update accessTokenSignal
              authStateService.refreshAuthState();

              // Retry original request with new token
              const retryReq = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
              return next(retryReq);
            }

            // If refresh failed, propagate original error
            return throwError(() => err);
          })
        );
      }

      return throwError(() => err);
    })
  );
};

async function fetchNewAccessToken(): Promise<string | null> {
  try {
    // Call backend refresh endpoint; cookie with refresh token will be sent automatically
  const base = environment.apiBaseUrl ?? location.origin;
  const resp = await fetch(`${base}/api/auth/refresh`, {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json' }
    });

    if (!resp.ok) return null;
    const body = await resp.json();
    return body?.accessToken ?? null;
  } catch {
    return null;
  }
}

/**
 * Determine if request should skip authentication.
 * Skip for:
 * - Auth endpoints (/api/auth/login, /api/auth/register)
 * - Public assets (/assets/*, /i18n/*)
 */
function shouldSkipAuth(url: string): boolean {
  // Skip auth endpoints
  if (url.includes('/api/auth/login') || url.includes('/api/auth/register')) {
    return true;
  }

  // Skip public assets and i18n
  if (url.includes('/assets/') || url.includes('/i18n/')) {
    return true;
  }

  return false;
}
