# 🔐 Sistema de Autenticación - SportPlanner

## ✅ Implementación Completada - Frontend llama a Backend .NET

### **Arquitectura**

```
Angular Frontend → .NET Backend → Supabase Auth
```

El frontend **NO** llama directamente a Supabase. Todo pasa por tu backend .NET.

### **Archivos Creados**

#### **Core Auth (8 archivos)**
- ✅ `src/app/core/auth/models/user.model.ts` - Modelos TypeScript + mapeo de API
- ✅ `src/app/core/auth/services/auth.service.ts` - Llama a `/api/auth/*` endpoints
- ✅ `src/app/core/auth/services/auth-state.service.ts` - Estado global con Signals
- ✅ `src/app/core/auth/services/token.service.ts` - Gestión de JWT en localStorage
- ✅ `src/app/core/auth/guards/auth.guard.ts` - Guard funcional
- ✅ `src/app/core/auth/guards/role.guard.ts` - Guard de roles
- ✅ `src/app/core/auth/interceptors/auth.interceptor.ts` - Inyecta Bearer token
- ✅ `src/app/core/auth/index.ts` - Barrel export

#### **Configuración**
- ✅ `src/environments/environment.ts` - Variables de entorno
- ✅ `src/app/app.config.ts` - Interceptor registrado
- ✅ `src/app/app.routes.ts` - Rutas protegidas con authGuard

#### **UI Actualizada**
- ✅ `src/app/features/auth/pages/auth-page/auth-page.ts` - Integrado con Supabase
- ✅ `src/app/features/auth/pages/auth-page/auth-page.html` - Formularios reactivos
- ✅ `src/app/features/auth/auth.routes.ts` - Lazy loading

---

## 🚀 Configuración Requerida

### **1. Backend .NET - YA CONFIGURADO ✅**

Tu backend ya tiene todo listo en `appsettings.json`:
- ✅ Supabase URL y Key
- ✅ `AuthController` con endpoints `/api/auth/login` y `/api/auth/register`
- ✅ `SupabaseAuthService` integrado

### **2. Frontend - Variables de Entorno**

`src/environments/environment.ts` ya está configurado:

\`\`\`typescript
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7103' // Tu backend .NET
};
\`\`\`

**Nota**: Todos los servicios usan `apiBaseUrl` + ruta del controller (ej: `/api/auth/login`)

### **3. CORS en Backend (Importante)**

Asegúrate de que tu `.NET API` permite peticiones desde Angular:

\`\`\`csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ...
app.UseCors(); // ANTES de app.UseAuthorization()
\`\`\`

---

## 📖 Uso en Componentes

### **1. Login/Register**

Ya está implementado en `auth-page`. Solo necesitas configurar las variables de entorno.

### **2. Proteger Rutas**

\`\`\`typescript
import { authGuard, roleGuard } from '@core/auth';

const routes: Routes = [
  {
    path: 'admin',
    loadComponent: () => import('./admin.component'),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['admin'] }
  }
];
\`\`\`

### **3. Acceder al Usuario Actual**

\`\`\`typescript
import { inject } from '@angular/core';
import { AuthStateService } from '@core/auth';

export class MyComponent {
  private authState = inject(AuthStateService);

  // Acceder al usuario
  user = this.authState.user; // Signal<User | null>
  
  // Verificar si está autenticado
  isAuthenticated = this.authState.isAuthenticated; // Signal<boolean>
  
  // Obtener token JWT
  accessToken = this.authState.accessToken; // Signal<string | null>
}
\`\`\`

### **4. Cerrar Sesión**

\`\`\`typescript
import { inject } from '@angular/core';
import { AuthService } from '@core/auth';

export class MyComponent {
  private authService = inject(AuthService);

  async logout() {
    await this.authService.signOut();
    // Automáticamente redirige a /auth/login
  }
}
\`\`\`

### **5. Llamadas HTTP Autenticadas**

El interceptor automáticamente añade el token Bearer a las peticiones:

\`\`\`typescript
// ❌ NO HAGAS ESTO
fetch('/api/teams', {
  headers: { 'Authorization': \`Bearer \${token}\` }
});

// ✅ HAZLO ASÍ (HttpClient lo hace automáticamente)
import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export class MyService {
  private http = inject(HttpClient);

  getTeams() {
    return this.http.get('/api/teams'); // JWT se añade automáticamente
  }
}
\`\`\`

---

## 🧪 Testing

### **Probar Login (Flujo Completo)**

1. **Inicia el backend .NET**:
   \`\`\`bash
   cd back/SportPlanner
   dotnet run --project src/SportPlanner.API
   \`\`\`
   
   Debería estar corriendo en: `https://localhost:7103`

2. **Inicia el frontend Angular**:
   \`\`\`bash
   cd front/SportPlanner
   npm start
   \`\`\`
   
   Debería estar corriendo en: `http://localhost:4200`

3. **Prueba el flujo**:
   - Ve a: http://localhost:4200/auth
   - Pestaña "Registrarse": Crea una cuenta nueva
   - O pestaña "Iniciar Sesión": Usa credenciales existentes

4. **Verifica en DevTools**:
   - **Network tab**: Verás `POST https://localhost:7103/api/auth/login`
   - **Response**: `{ userId, firstName, lastName, email, role, accessToken }`
   - **Console**: "Login successful: tu@email.com"

5. **Verificar redirección**: Deberías ser redirigido a `/dashboard`

### **Verificar JWT Token**

1. Abre DevTools > Application > Local Storage
2. Busca `sportplanner-access-token`
3. Copia el token y pégalo en https://jwt.io
4. Verifica que contenga:
   - `sub`: User ID (UUID de Supabase)
   - `email`: Email del usuario
   - `role`: admin o el rol configurado

---

## 🔒 Seguridad

### **Implementado**

- ✅ JWT almacenado en `localStorage` (solo browser)
- ✅ SSR-safe: Detecta entorno browser vs server
- ✅ Auto-refresh de tokens (manejado por Supabase)
- ✅ Interceptor añade Bearer token automáticamente
- ✅ Guards protegen rutas privadas
- ✅ Session persistence activada

### **Pendiente (Producción)**

- ⚠️ Configurar HTTPS en producción
- ⚠️ Habilitar email confirmation en Supabase
- ⚠️ Configurar rate limiting en backend
- ⚠️ Añadir refresh token rotation (opcional)

---

## 🐛 Troubleshooting

### **Error: "Failed to fetch" o CORS error**

**Causa**: Backend no está corriendo o CORS no está configurado  
**Solución**: 
1. Verifica que el backend esté corriendo en `https://localhost:7000`
2. Añade CORS en `Program.cs` (ver sección de configuración arriba)

### **Error: 401 Unauthorized**

**Causa**: Credenciales incorrectas  
**Solución**: Verifica email/password en Supabase Dashboard > Authentication > Users

### **Error: 409 Conflict en register**

**Causa**: Email ya registrado  
**Solución**: Usa otro email o inicia sesión con el existente

### **Guard no redirige al login**

**Causa**: Token existe pero es inválido  
**Solución**: 
1. Limpia localStorage: `localStorage.clear()`
2. Recarga la página
3. Intenta login nuevamente

---

## 📦 Dependencias

**Frontend**: Solo Angular HttpClient (no se requiere @supabase/supabase-js)  
**Backend**: Ya tiene Supabase NuGet package instalado

---

## 🎯 Próximos Pasos

### **Paso 3: Backend Integration** (Pendiente)

1. Crear `ICurrentUserService` en .NET
2. Extraer `UserId` del claim `sub` del JWT
3. Añadir `[Authorize]` a controllers
4. Validar roles desde metadata de Supabase

### **Paso 4: Features Adicionales** (Opcional)

- [ ] Reset password flow
- [ ] Email confirmation flow
- [ ] User profile editing
- [ ] Avatar upload
- [ ] Remember me functionality
- [ ] 2FA (MFA)

---

## 📝 Validación contra .clinerules

- ✅ **SSR-safe**: Usa `isPlatformBrowser()` en todos los servicios
- ✅ **Standalone**: No usa NgModules
- ✅ **Signals**: Estado reactivo con `signal()`, `computed()`
- ✅ **Functional**: Guards e interceptors son funciones, no clases
- ✅ **Nomenclatura**: PascalCase para interfaces, camelCase para propiedades
- ✅ **Documentación**: JSDoc en métodos públicos
- ✅ **Lazy loading**: Rutas con `loadChildren` y `loadComponent`
- ✅ **Tailwind**: UI usa clases de Tailwind (sin Angular Material)

---

## 📞 Soporte

Si encuentras problemas:

1. Verifica logs en DevTools Console
2. Revisa Network tab para ver peticiones fallidas
3. Verifica que Supabase esté configurado correctamente
4. Revisa este README para configuración

---

**✨ Sistema de autenticación listo para usar! Solo configura `environment.ts` y prueba el login.**
