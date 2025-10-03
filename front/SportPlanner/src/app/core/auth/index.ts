/**
 * Public exports for auth module.
 * Import from this barrel file to access auth functionality.
 * 
 * Example:
 * ```typescript
 * import { AuthService, authGuard, User } from '@core/auth';
 * ```
 */

// Services
export { AuthService } from './services/auth.service';
export { AuthStateService } from './services/auth-state.service';
export { TokenService } from './services/token.service';

// Guards
export { authGuard } from './guards/auth.guard';
export { roleGuard } from './guards/role.guard';

// Interceptors
export { authInterceptor } from './interceptors/auth.interceptor';

// Models
export type { 
  User, 
  UserRole, 
  AuthState, 
  LoginCredentials, 
  RegisterPayload,
  PasswordResetRequest,
  PasswordUpdatePayload,
  ApiAuthResponse
} from './models/user.model';

export { mapApiResponseToUser } from './models/user.model';
