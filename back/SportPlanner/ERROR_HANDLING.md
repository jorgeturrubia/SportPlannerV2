# 🔥 Manejo de Errores - Backend

## 📋 Cambios Implementados

### **Problema Anterior**
❌ Los errores de Supabase se "tragaban" silenciosamente  
❌ El frontend recibía `null` sin información del error  
❌ No se podían mostrar mensajes específicos al usuario  

### **Solución Implementada**
✅ Los errores de Supabase se propagan al controller  
✅ El controller retorna respuestas HTTP con mensajes claros  
✅ El frontend recibe información detallada del error  

---

## 🔧 Arquitectura de Errores

```
Supabase Auth Error
    ↓
Handler (Application Layer)
    ↓ throw UnauthorizedAccessException / InvalidOperationException
Controller (API Layer)
    ↓ catch y retorna HTTP status + mensaje
Frontend (Angular)
    ↓ muestra mensaje al usuario
```

---

## 📁 Archivos Modificados

### **1. LoginUserQueryHandler.cs**

**ANTES:**
```csharp
catch (Exception)
{
    // Supabase auth failed
    return null;  // ❌ Se pierde la información del error
}
```

**DESPUÉS:**
```csharp
catch (Exception ex)
{
    // Propagate Supabase auth error with details
    throw new UnauthorizedAccessException($"Authentication failed: {ex.Message}", ex);
}
```

✅ **Ventajas:**
- El mensaje de Supabase se preserva
- El tipo de excepción es semántico (`UnauthorizedAccessException`)
- El controller puede distinguir entre tipos de error

---

### **2. RegisterUserCommandHandler.cs**

**ANTES:**
```csharp
// Sin try-catch
var session = await _authService.SignUpAsync(request.Email, request.Password);

if (session?.User is null)
{
    return null;  // ❌ No sabemos por qué falló
}
```

**DESPUÉS:**
```csharp
try
{
    var session = await _authService.SignUpAsync(request.Email, request.Password);

    if (session?.User is null)
    {
        throw new InvalidOperationException("Failed to create user in Supabase");
    }
    
    // ... resto del código
}
catch (Exception ex)
{
    // Propagate Supabase registration error with details
    throw new InvalidOperationException($"Registration failed: {ex.Message}", ex);
}
```

✅ **Ventajas:**
- Detectamos si Supabase retorna `null` (email ya existe, contraseña débil, etc.)
- El mensaje de error original de Supabase se incluye
- El tipo `InvalidOperationException` es específico para operaciones fallidas

---

### **3. AuthController.cs**

**ANTES (Login):**
```csharp
var result = await _mediator.Send(query);

if (result is null)
{
    return Unauthorized("Invalid email or password");  // ❌ Siempre el mismo mensaje
}

return Ok(result);
```

**DESPUÉS (Login):**
```csharp
try
{
    var query = new LoginUserQuery(request.Email, request.Password);
    var result = await _mediator.Send(query);

    if (result is null)
    {
        return Unauthorized(new { error = "Invalid email or password" });
    }

    return Ok(result);
}
catch (UnauthorizedAccessException ex)
{
    // Return Supabase auth error details
    return Unauthorized(new { error = ex.Message });  // ✅ Error específico de Supabase
}
catch (Exception ex)
{
    return BadRequest(new { error = $"Login error: {ex.Message}" });
}
```

**ANTES (Register):**
```csharp
var result = await _mediator.Send(command);
return Ok(result);  // ❌ No maneja errores
```

**DESPUÉS (Register):**
```csharp
try
{
    var command = new RegisterUserCommand(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Password);

    var result = await _mediator.Send(command);
    return Ok(result);
}
catch (InvalidOperationException ex)
{
    // Return Supabase error details
    return BadRequest(new { error = ex.Message });  // ✅ Error específico
}
catch (Exception ex)
{
    return BadRequest(new { error = $"Registration error: {ex.Message}" });
}
```

---

## 📊 Respuestas HTTP

### **Login Exitoso**
```json
HTTP 200 OK
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "role": "Admin",
  "accessToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

### **Login - Credenciales Inválidas**
```json
HTTP 401 Unauthorized
{
  "error": "Authentication failed: Invalid login credentials"
}
```

### **Login - Email no confirmado**
```json
HTTP 401 Unauthorized
{
  "error": "Authentication failed: Email not confirmed"
}
```

### **Register Exitoso**
```json
HTTP 200 OK
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "firstName": "María",
  "lastName": "García",
  "email": "maria@example.com",
  "role": "Admin",
  "accessToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

### **Register - Email ya existe**
```json
HTTP 400 Bad Request
{
  "error": "Registration failed: User already registered"
}
```

### **Register - Contraseña débil**
```json
HTTP 400 Bad Request
{
  "error": "Registration failed: Password should be at least 6 characters"
}
```

### **Register - Email inválido**
```json
HTTP 400 Bad Request
{
  "error": "Registration failed: Unable to validate email address: invalid format"
}
```

---

## 🎯 Errores Comunes de Supabase

| Error Supabase | HTTP Status | Mensaje |
|----------------|-------------|---------|
| Credenciales inválidas | 401 | `Authentication failed: Invalid login credentials` |
| Email no confirmado | 401 | `Authentication failed: Email not confirmed` |
| Usuario ya existe | 400 | `Registration failed: User already registered` |
| Contraseña débil | 400 | `Registration failed: Password should be at least 6 characters` |
| Email inválido | 400 | `Registration failed: Unable to validate email address: invalid format` |
| Rate limit excedido | 429 | `Authentication failed: Email rate limit exceeded` |

---

## 🔄 Integración con Frontend

### **AuthService (Angular)**

El `mapHttpError()` debe actualizarse para manejar el nuevo formato:

```typescript
private mapHttpError(error: HttpErrorResponse): string {
  // El backend ahora retorna { error: "mensaje" }
  if (error.error?.error) {
    return error.error.error;  // ✅ Usar mensaje del backend
  }

  // Fallback a mensajes genéricos
  switch (error.status) {
    case 401:
      return 'Email o contraseña incorrectos';
    case 400:
      return 'Error en el registro. Verifica los datos.';
    default:
      return 'Error de conexión con el servidor';
  }
}
```

### **Ejemplo de Uso**

```typescript
// Login
try {
  const user = await this.authService.signIn(email, password);
  // Éxito
} catch (error) {
  // error.message contendrá el error específico de Supabase
  console.error('Login error:', error.message);
  // Ejemplo: "Authentication failed: Invalid login credentials"
}

// Register
try {
  const user = await this.authService.signUp(name, email, password);
  // Éxito
} catch (error) {
  // error.message contendrá el error específico de Supabase
  console.error('Register error:', error.message);
  // Ejemplo: "Registration failed: User already registered"
}
```

---

## ✅ Beneficios de Esta Implementación

1. **🎯 Mensajes Específicos**: El usuario sabe exactamente qué salió mal
2. **🔍 Debugging Facilitado**: Los logs contienen información detallada
3. **🌐 Internacionalización**: Fácil traducir mensajes específicos en el frontend
4. **📊 Monitoreo**: Permite identificar patrones de errores (rate limiting, emails no confirmados, etc.)
5. **🔒 Seguridad**: No expone información sensible del sistema, solo errores de negocio

---

## 🧪 Testing

### **Casos de Prueba**

1. **Login con credenciales inválidas**
   - Input: Email existente + contraseña incorrecta
   - Esperado: 401 con mensaje "Invalid login credentials"

2. **Login con email no confirmado**
   - Input: Email registrado pero no confirmado
   - Esperado: 401 con mensaje "Email not confirmed"

3. **Register con email existente**
   - Input: Email ya registrado en Supabase
   - Esperado: 400 con mensaje "User already registered"

4. **Register con contraseña débil**
   - Input: Contraseña de menos de 6 caracteres
   - Esperado: 400 con mensaje "Password should be at least 6 characters"

5. **Register con email inválido**
   - Input: Email sin formato válido (ej: "notanemail")
   - Esperado: 400 con mensaje "Unable to validate email address"

---

## 🚀 Próximos Pasos

1. ✅ **Implementado**: Propagar errores de Supabase
2. ✅ **Implementado**: Retornar mensajes claros en controllers
3. 🔄 **Siguiente**: Actualizar AuthService en Angular para usar `error.error.error`
4. 📝 **Siguiente**: Agregar tests unitarios para cada caso de error
5. 🌐 **Futuro**: Agregar internacionalización de mensajes de error

---

**🎉 Los errores de Supabase ahora son visibles y accionables!**
