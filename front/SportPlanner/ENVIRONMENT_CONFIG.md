# ✅ Actualización de Configuración - Puerto y Variables de Entorno

## 📋 Cambios Realizados

### **1. Environments Actualizados**

**Cambio de `apiUrl` a `apiBaseUrl` y puerto correcto:**

```typescript
// ANTES
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000'
};

// DESPUÉS
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7103'  // ✅ Puerto correcto del backend
};
```

**Archivos modificados:**
- ✅ `src/environments/environment.ts`
- ✅ `src/environments/environment.development.ts`

---

### **2. Servicios Actualizados**

Todos los servicios ahora usan `apiBaseUrl` + ruta del controller:

#### **AuthService**
```typescript
private apiBaseUrl = environment.apiBaseUrl;

// Login
this.http.post(`${this.apiBaseUrl}/api/auth/login`, ...)

// Register  
this.http.post(`${this.apiBaseUrl}/api/auth/register`, ...)
```

#### **TeamsService**
```typescript
private apiBaseUrl = environment.apiBaseUrl;

// Get teams
this.http.get(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams`)

// Get team
this.http.get(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`)

// Create team
this.http.post(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams`, payload)

// Update team
this.http.put(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`, payload)

// Delete team
this.http.delete(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`)
```

**Archivos modificados:**
- ✅ `src/app/core/auth/services/auth.service.ts`
- ✅ `src/app/features/dashboard/services/teams.service.ts`

---

## 🎯 Ventajas de Este Patrón

✅ **Centralizado**: Un solo lugar para cambiar la URL del backend (`environment.ts`)  
✅ **Consistente**: Todos los servicios siguen el mismo patrón  
✅ **Mantenible**: Fácil cambiar entre dev, staging, production  
✅ **Claro**: `apiBaseUrl` + ruta del controller es más explícito  

---

## 🔧 Configuración por Ambiente

### **Development** (actual)
```typescript
// environment.development.ts
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7103'
};
```

### **Production** (ejemplo)
```typescript
// environment.ts
export const environment = {
  production: true,
  apiBaseUrl: 'https://api.sportplanner.com'
};
```

### **Staging** (ejemplo)
```typescript
// environment.staging.ts
export const environment = {
  production: false,
  apiBaseUrl: 'https://api-staging.sportplanner.com'
};
```

---

## 🧪 Verificación

### **Backend corriendo en puerto correcto:**
```bash
cd back/SportPlanner
dotnet run --project src/SportPlanner.API
```

Verifica en la consola:
```
Now listening on: https://localhost:7103
Now listening on: http://localhost:5112
```

### **Frontend apunta al puerto correcto:**

1. Abre: `src/environments/environment.ts`
2. Verifica: `apiBaseUrl: 'https://localhost:7103'`
3. Inicia frontend: `npm start`
4. Abre DevTools > Network
5. Haz login
6. Verifica que las llamadas van a: `https://localhost:7103/api/auth/login`

---

## ✅ Estado Final

| Componente | Puerto/URL | Estado |
|------------|-----------|--------|
| Backend .NET | `https://localhost:7103` | ✅ Correcto |
| Frontend Angular | `http://localhost:4200` | ✅ Correcto |
| Environment var | `apiBaseUrl` | ✅ Actualizado |
| AuthService | Usa `apiBaseUrl` | ✅ Actualizado |
| TeamsService | Usa `apiBaseUrl` | ✅ Actualizado |
| Errores | 0 | ✅ Sin errores |

---

## 📝 Patrón de Uso para Nuevos Servicios

Cuando crees un nuevo servicio, sigue este patrón:

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MiNuevoService {
  private http = inject(HttpClient);
  private apiBaseUrl = environment.apiBaseUrl;  // ✅ Importar base URL

  async getData() {
    return firstValueFrom(
      this.http.get(`${this.apiBaseUrl}/api/mi-controller/endpoint`)  // ✅ Concatenar
    );
  }
}
```

---

**🚀 Configuración lista para desarrollo y producción!**
