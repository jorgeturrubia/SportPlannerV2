# Security Standards

## Autenticación y Autorización
- **JWT Validation**: Validar siempre JWT vía JWKS de Supabase en backend y frontend
- **Token Storage**: NUNCA almacenar tokens ni contraseñas en localStorage/sessionStorage
- **Session Management**: Usar httpOnly cookies para tokens sensibles
- **Refresh Tokens**: Implementar rotación automática de tokens

## Validación de Entrada
- **Client-side**: Validación para UX, NUNCA para seguridad
- **Server-side**: Validación estricta de todos los inputs
- **Sanitización**: Limpiar datos antes de procesamiento
- **SQL Injection**: Usar parámetros preparados, nunca concatenación

## Gestión de Secrets
- **Environment Variables**: Todos los secrets en variables de entorno
- **Rotación**: Rotar claves siguiendo mejores prácticas (cada 90 días)
- **Acceso**: Principio de menor privilegio
- **Versionado**: NUNCA commitear secrets en el repositorio

## HTTPS y Comunicación
- **TLS**: Forzar HTTPS en producción
- **CORS**: Configurar dominios específicos, nunca wildcard en producción
- **Headers**: Implementar security headers (CSP, HSTS, etc.)
- **API Security**: Rate limiting y throttling

## Específico de SportPlanner
```typescript
// Ejemplo: JWT Validation Middleware
export class JwtValidationMiddleware {
  async validate(token: string): Promise<boolean> {
    try {
      // Validar contra JWKS endpoint
      const jwks = await this.getJWKS();
      const decoded = jwt.verify(token, jwks);
      
      // Verificar claims específicos
      return this.validateClaims(decoded);
    } catch (error) {
      return false;
    }
  }
}
```

## Datos Sensibles del Dominio
- **Datos de Salud**: Encriptación en reposo y tránsito
- **Información Personal**: Cumplir GDPR/LOPD
- **Datos de Entrenamiento**: Anonimización para analytics
- **Dispositivos**: Validar integridad de datos de sensores

## Auditoría y Logging
- **Access Logs**: Registrar accesos a datos sensibles
- **Failed Attempts**: Log de intentos de autenticación fallidos
- **Data Changes**: Auditoría de cambios en datos críticos
- **Security Events**: Alertas automáticas para eventos sospechosos

## OWASP Top 10 Compliance
- **Injection**: Prevención de SQL, NoSQL, Command injection
- **Broken Authentication**: Implementación robusta de autenticación
- **Sensitive Data Exposure**: Encriptación de datos sensibles
- **XXE**: Validación de XML inputs
- **Broken Access Control**: Autorización granular
- **Security Misconfiguration**: Hardening de infraestructura
- **XSS**: Sanitización de outputs
- **Insecure Deserialization**: Validación de datos deserializados
- **Known Vulnerabilities**: Mantenimiento de dependencias
- **Insufficient Logging**: Monitoreo comprehensivo
