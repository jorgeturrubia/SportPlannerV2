# Supabase JWT Authentication

## JWT Configuration
- **Issuer (iss)**: Verificar que corresponda a tu proyecto Supabase
- **Audience (aud)**: Validar audiencia correcta configurada
- **Claims**: Usar claims personalizados para roles y permisos
- **Expiration**: Configurar tiempo de vida apropiado (15-60 min para access tokens)

```typescript
// Configuración JWT
const jwtConfig = {
  issuer: 'https://your-project.supabase.co/auth/v1',
  audience: 'authenticated',
  algorithms: ['HS256'],
  clockTolerance: 10
};
```

## JWKS Validation
- **Endpoint JWKS**: Validar contra `/.well-known/jwks.json`
- **Cache Duration**: NO cachear más de 10 minutos
- **Key Rotation**: Manejar rotación automática de claves
- **Fallback**: Tener estrategia de fallback si JWKS no está disponible

```typescript
// JWT Validation Service
@Injectable()
export class JwtValidationService {
  private jwksCache = new Map<string, any>();
  private readonly CACHE_DURATION = 10 * 60 * 1000; // 10 minutes

  async validateToken(token: string): Promise<boolean> {
    try {
      const jwks = await this.getJWKS();
      const decoded = jwt.verify(token, jwks);
      return this.validateClaims(decoded);
    } catch (error) {
      console.error('JWT validation failed:', error);
      return false;
    }
  }

  private async getJWKS() {
    const cacheKey = 'jwks';
    const cached = this.jwksCache.get(cacheKey);
    
    if (cached && Date.now() - cached.timestamp < this.CACHE_DURATION) {
      return cached.data;
    }

    const response = await fetch(`${environment.supabaseUrl}/auth/v1/.well-known/jwks.json`);
    const jwks = await response.json();
    
    this.jwksCache.set(cacheKey, {
      data: jwks,
      timestamp: Date.now()
    });
    
    return jwks;
  }
}
```

## Role-Based Access Control
- **Claims Management**: Roles gestionados por claims en JWT
- **Feature-based Authorization**: Separar autenticación de autorización por feature
- **Granular Permissions**: Permisos específicos por recurso

```typescript
// User Roles para SportPlanner
export enum UserRole {
  ATHLETE = 'athlete',
  COACH = 'coach',
  ADMIN = 'admin',
  NUTRITIONIST = 'nutritionist'
}

// Claims interface
interface UserClaims {
  sub: string;
  email: string;
  role: UserRole;
  permissions: string[];
  athlete_id?: string;
  coach_id?: string;
}
```

## Authentication Guard
```typescript
@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  async canActivate(route: ActivatedRouteSnapshot): Promise<boolean> {
    const token = await this.authService.getToken();
    
    if (!token || !await this.authService.validateToken(token)) {
      this.router.navigate(['/login']);
      return false;
    }

    // Check role-based permissions if required
    const requiredRole = route.data?.['role'];
    if (requiredRole) {
      const userRole = this.authService.getUserRole();
      return this.authService.hasRole(requiredRole);
    }

    return true;
  }
}
```

## Supabase Integration
```typescript
// Auth Service con Supabase
@Injectable()
export class AuthService {
  private supabase = createClient(environment.supabaseUrl, environment.supabaseKey);

  async signIn(email: string, password: string) {
    const { data, error } = await this.supabase.auth.signInWithPassword({
      email,
      password
    });

    if (error) throw error;
    
    // Validar JWT después del login
    const isValid = await this.validateToken(data.session?.access_token);
    if (!isValid) throw new Error('Invalid token received');
    
    return data;
  }

  async getSession() {
    const { data } = await this.supabase.auth.getSession();
    return data.session;
  }

  async refreshSession() {
    const { data, error } = await this.supabase.auth.refreshSession();
    if (error) throw error;
    return data;
  }
}
```

## Token Storage Strategy
- **Memory Storage**: Para aplicaciones SPA seguras
- **HttpOnly Cookies**: Para aplicaciones con servidor
- **Refresh Token Rotation**: Implementar rotación automática

```typescript
// Secure Token Storage
@Injectable()
export class TokenStorageService {
  private accessToken: string | null = null;
  private refreshToken: string | null = null;

  setTokens(access: string, refresh: string) {
    this.accessToken = access;
    this.refreshToken = refresh;
    // NO usar localStorage para tokens sensibles
  }

  getAccessToken(): string | null {
    return this.accessToken;
  }

  clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
  }
}
```

## Error Handling
```typescript
// JWT Error Handler
export class JwtErrorHandler {
  handleError(error: any): Observable<never> {
    if (error.name === 'JsonWebTokenError') {
      // Token malformado
      this.authService.logout();
    } else if (error.name === 'TokenExpiredError') {
      // Token expirado - intentar refresh
      return this.authService.refreshToken();
    } else if (error.name === 'NotBeforeError') {
      // Token no válido aún
      console.warn('Token not active yet');
    }
    
    return throwError(error);
  }
}
```
