/**
 * User model representing authenticated user.
 * Maps to Supabase auth.users table + metadata.
 */
export interface User {
  id: string; // Supabase user ID (UUID)
  email: string;
  emailConfirmed: boolean;
  role: UserRole;
  createdAt: Date;
  lastSignIn: Date | null;
  
  // Optional profile metadata from Supabase user_metadata
  displayName?: string;
  avatarUrl?: string;
}

/**
 * User roles for authorization.
 * Currently only 'admin' is implemented.
 */
export type UserRole = 'admin';

/**
 * Authentication state wrapper.
 */
export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  accessToken: string | null;
}

/**
 * Login credentials.
 */
export interface LoginCredentials {
  email: string;
  password: string;
}

/**
 * Registration payload.
 */
export interface RegisterPayload {
  email: string;
  password: string;
  displayName?: string;
}

/**
 * Password reset request.
 */
export interface PasswordResetRequest {
  email: string;
}

/**
 * Password update payload.
 */
export interface PasswordUpdatePayload {
  newPassword: string;
}

/**
 * Backend API auth response (from .NET AuthResponse DTO).
 */
export interface ApiAuthResponse {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  accessToken: string;
}

/**
 * Map API response to User model.
 */
export function mapApiResponseToUser(response: ApiAuthResponse): User {
  return {
    id: response.userId,
    email: response.email,
    emailConfirmed: true, // Assume confirmed if login successful
    role: response.role.toLowerCase() as UserRole,
    createdAt: new Date(),
    lastSignIn: new Date(),
    displayName: `${response.firstName} ${response.lastName}`.trim(),
    avatarUrl: undefined
  };
}
