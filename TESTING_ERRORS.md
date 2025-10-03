# 🧪 Testing de Manejo de Errores

## 📋 Guía de Pruebas para Errores de Supabase

### **Configuración Inicial**

1. **Backend corriendo:**
   ```powershell
   cd back/SportPlanner
   dotnet run --project src/SportPlanner.API
   ```

2. **Frontend corriendo:**
   ```powershell
   cd front/SportPlanner
   npm start
   ```

3. **Abrir DevTools:**
   - `F12` o `Ctrl+Shift+I`
   - Ir a la pestaña **Network**
   - Ir a la pestaña **Console**

---

## 🔴 Caso 1: Login con Credenciales Inválidas

### **Objetivo**
Verificar que el error de Supabase "Invalid login credentials" se muestra al usuario.

### **Pasos**
1. Navega a: `http://localhost:4200/auth`
2. Ingresa:
   - Email: `test@example.com`
   - Password: `wrongpassword123`
3. Click en **Iniciar Sesión**

### **Resultado Esperado**

**Network Tab (Request):**
```
POST https://localhost:7103/api/auth/login
Status: 401 Unauthorized

Request Payload:
{
  "email": "test@example.com",
  "password": "wrongpassword123"
}

Response:
{
  "error": "Authentication failed: Invalid login credentials"
}
```

**UI:**
```
❌ Authentication failed: Invalid login credentials
```

**Console:**
```
Error: Authentication failed: Invalid login credentials
```

---

## 🟡 Caso 2: Register con Email Existente

### **Objetivo**
Verificar que el error "User already registered" se muestra correctamente.

### **Pasos**
1. Primero registra un usuario:
   - Nombre: `Test User`
   - Email: `existing@example.com`
   - Password: `password123`

2. Intenta registrar el mismo email nuevamente:
   - Nombre: `Another User`
   - Email: `existing@example.com`
   - Password: `differentpass123`

### **Resultado Esperado**

**Network Tab (Request):**
```
POST https://localhost:7103/api/auth/register
Status: 400 Bad Request

Request Payload:
{
  "firstName": "Another",
  "lastName": "User",
  "email": "existing@example.com",
  "password": "differentpass123"
}

Response:
{
  "error": "Registration failed: User already registered"
}
```

**UI:**
```
❌ Registration failed: User already registered
```

---

## 🟢 Caso 3: Register con Contraseña Débil

### **Objetivo**
Verificar validación de contraseña por Supabase.

### **Pasos**
1. En el formulario de registro:
   - Nombre: `Weak Pass User`
   - Email: `weakpass@example.com`
   - Password: `12345` (menos de 6 caracteres)

### **Resultado Esperado**

**Network Tab:**
```
POST https://localhost:7103/api/auth/register
Status: 400 Bad Request

Response:
{
  "error": "Registration failed: Password should be at least 6 characters"
}
```

**UI:**
```
❌ Registration failed: Password should be at least 6 characters
```

---

## 🔵 Caso 4: Register con Email Inválido

### **Objetivo**
Verificar validación de formato de email.

### **Pasos**
1. En el formulario de registro:
   - Nombre: `Invalid Email`
   - Email: `notanemail` (sin @ ni dominio)
   - Password: `password123`

### **Resultado Esperado**

**Network Tab:**
```
POST https://localhost:7103/api/auth/register
Status: 400 Bad Request

Response:
{
  "error": "Registration failed: Unable to validate email address: invalid format"
}
```

**UI:**
```
❌ Registration failed: Unable to validate email address: invalid format
```

---

## 🟣 Caso 5: Login con Email No Confirmado

### **Objetivo**
Verificar error cuando el email requiere confirmación.

### **Pre-requisito**
- Supabase debe tener habilitada la confirmación de email en:
  - Dashboard > Authentication > Email Auth > Confirm email

### **Pasos**
1. Registra un nuevo usuario
2. NO confirmes el email (no hagas click en el link del email)
3. Intenta hacer login con ese email

### **Resultado Esperado**

**Network Tab:**
```
POST https://localhost:7103/api/auth/login
Status: 401 Unauthorized

Response:
{
  "error": "Authentication failed: Email not confirmed"
}
```

**UI:**
```
❌ Authentication failed: Email not confirmed
```

---

## ⚡ Caso 6: Rate Limiting (Bonus)

### **Objetivo**
Verificar manejo de rate limit de Supabase.

### **Pasos**
1. Intenta hacer login **10+ veces seguidas** con credenciales inválidas
2. Supabase bloqueará temporalmente los intentos

### **Resultado Esperado**

**Network Tab:**
```
POST https://localhost:7103/api/auth/login
Status: 429 Too Many Requests

Response:
{
  "error": "Authentication failed: Email rate limit exceeded"
}
```

**UI:**
```
❌ Demasiados intentos. Espera unos minutos.
```

---

## 📊 Checklist de Verificación

Marca cada caso cuando lo hayas probado:

- [ ] ✅ Login con credenciales inválidas → 401 + mensaje específico
- [ ] ✅ Register con email existente → 400 + "User already registered"
- [ ] ✅ Register con contraseña débil → 400 + "Password should be at least 6 characters"
- [ ] ✅ Register con email inválido → 400 + "invalid format"
- [ ] ✅ Login con email no confirmado → 401 + "Email not confirmed"
- [ ] ✅ Rate limiting → 429 + mensaje de espera

---

## 🔍 Debugging

### **Si no ves el error específico:**

1. **Verifica que el backend esté actualizado:**
   ```powershell
   cd back/SportPlanner
   dotnet build
   dotnet run --project src/SportPlanner.API
   ```

2. **Verifica la respuesta en Network tab:**
   - ¿El response tiene `{ error: "mensaje" }`?
   - Si es `null` o diferente, el backend no está usando el código nuevo

3. **Verifica el frontend:**
   ```typescript
   // En auth.service.ts debería tener:
   if (error.error?.error) {
     return new Error(error.error.error);  // ✅ Correcto
   }
   ```

4. **Verifica logs del backend:**
   - Los logs deberían mostrar las excepciones con mensajes de Supabase

---

## 🎯 Errores Comunes

### **Error 1: CORS**
```
Access to XMLHttpRequest at 'https://localhost:7103/api/auth/login' 
from origin 'http://localhost:4200' has been blocked by CORS policy
```

**Solución:**
- Verifica que `Program.cs` tenga configurado CORS:
  ```csharp
  app.UseCors("AllowAll");
  ```

### **Error 2: Certificate SSL**
```
NET::ERR_CERT_AUTHORITY_INVALID
```

**Solución:**
- En Chrome/Edge: Click en "Avanzado" → "Continuar de todos modos"
- O usa HTTP en development: `http://localhost:5112` (verifica puerto en launchSettings.json)

### **Error 3: Backend no responde**
```
GET http://localhost:7103/api/auth/login net::ERR_CONNECTION_REFUSED
```

**Solución:**
- Verifica que el backend esté corriendo
- Verifica el puerto correcto en `environment.ts`:
  ```typescript
  apiBaseUrl: 'https://localhost:7103'  // ✅ Correcto
  ```

---

## 📝 Test Manual Script

Copia y pega en la consola del navegador para probar todos los casos:

```javascript
// Test 1: Invalid credentials
fetch('https://localhost:7103/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email: 'test@test.com', password: 'wrong' })
})
.then(r => r.json())
.then(console.log)
.catch(console.error);

// Test 2: Weak password
fetch('https://localhost:7103/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ 
    firstName: 'Test',
    lastName: 'User',
    email: 'newuser@test.com',
    password: '123'  // Muy corta
  })
})
.then(r => r.json())
.then(console.log)
.catch(console.error);

// Test 3: Invalid email format
fetch('https://localhost:7103/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ 
    firstName: 'Test',
    lastName: 'User',
    email: 'notanemail',  // Sin formato válido
    password: 'password123'
  })
})
.then(r => r.json())
.then(console.log)
.catch(console.error);
```

---

**🎉 Guía de testing completa! Sigue estos pasos para validar el manejo de errores.**
