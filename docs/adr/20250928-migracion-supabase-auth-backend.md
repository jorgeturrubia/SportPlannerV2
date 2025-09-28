# ADR-002: Migración Auth Backend Custom → Supabase Auth

**Fecha**: 2025-09-28

**Estado**: Aceptada

**Categoría**: SEC

**Tags**: authentication, security, supabase, jwt, migration

## Contexto

El backend actual implementa un sistema de autenticación custom con JWT usando SymmetricSecurityKey, lo cual viola las reglas de seguridad definidas en `.clinerules/13-supabase-jwt.md`. Esta implementación es incompatible con Supabase Auth y representa un riesgo de seguridad al no utilizar las claves públicas de Supabase para validación JWT.

### Problema

- **Violación de Reglas**: Uso de SymmetricSecurityKey en lugar de RS256 con validación JWKS
- **Incompatibilidad**: Sistema custom no integra con Supabase Auth
- **Riesgo de Seguridad**: No validación proper de tokens contra el issuer de Supabase
- **Complejidad**: Mantenimiento de servicio de auth custom innecesario

### Antecedentes

Inicialmente se implementó auth custom siguiendo un approach de "full control", pero esto resultó incompatible con la arquitectura recomendada por Supabase. Después de investigación y actualización de `.clinerules/13-supabase-jwt.md`, se determinó que el approach correcto es usar Authority + Audience (OIDC standard) donde .NET IdentityModel maneja automáticamente la descarga de JWKS.

## Opciones Consideradas

### Opción 1: Mantener Auth Custom + Supabase Integration

**Descripción**: Mantener el servicio actual pero agregar integración manual con Supabase solo para registro/login, manteniendo el custom JWT.

**Pros**:
- Menos cambios en el código existente
- Control total sobre generación de tokens

**Contras**:
- Viola reglas de seguridad (SymmetricSecurityKey no permitido)
- No aprovecha las ventajas de Supabase Auth (SCIM, MFA, etc.)
- Mayor complejidad de mantenimiento
- No compliance con standards OIDC/JWKS

**Estimación de Esfuerzo**: Medio

### Opción 2: Migración Completa a Supabase Auth (Authority + Audience)

**Descripción**: Reemplazar completamente el sistema custom por Supabase Auth usando el patrón Authority + Audience recomendado, donde .NET automáticamente descarga y valida JWKS.

**Pros**:
- Compliance total con reglas de seguridad
- Aprovecha features avanzadas de Supabase (MFA, password policies, etc.)
- Simplificación del código (menos mantenimiento)
- Standard OIDC compliance

**Contras**:
- Cambio significativo en la arquitectura
- Reaprendizaje del equipo en Supabase Auth patterns
- Dependencia de servicio externo

**Estimación de Esfuerzo**: Alto

### Opción 3: Auth Custom Mejorado con RS256

**Descripción**: Mantener el servicio custom pero migrar a RS256 con JWKS manual implementado.

**Pros**:
- Mantiene control sobre el servicio
- Implementa RS256 correctamente

**Contras**:
- Aún viola rules de Supabase Auth exclusivo
- Complejidad adicional innecesaria
- No integra con ecosystem de Supabase

**Estimación de Esfuerzo**: Alto

## Requisitos No Funcionales Evaluados

- **Seguridad**: Opción 2 es la más segura al usar validation automática JWKS
- **Rendimiento**: Todas las opciones similares, Opción 2 puede ser más eficiente por caching automático de keys
- **Mantenibilidad**: Opción 2 significativamente mejor (menos código custom)
- **Escalabilidad**: Opción 2 mejor integración con infra de Supabase
- **Compatibilidad**: Solo Opción 2 es 100% compatible con Supabase Auth

## Decisión

### Opción Elegida: **Opción 2 - Migración Completa a Supabase Auth (Authority + Audience)**

Se elige esta opción porque garantiza compliance total con las reglas de seguridad de SportPlanner y aprovecha las mejores prácticas de OIDC. El approach Authority + Audience es el recomendado por Supabase y Microsoft para .NET, proporcionando validación automática de JWKS sin código custom complejo.

## Consecuencias

### Impacto Positivo

- **Compliance de Seguridad**: Elimina todas las violaciones actuales
- **Reducción de Código**: ~60% menos líneas de código relacionadas con auth
- **Mantenibilidad**: Servicio standard con soporte oficial
- **Features Avanzadas**: MFA, password policies, account recovery automático

### Impacto Negativo

- **Cambio Arquitectónico**: Reaprendizaje del equipo requerido
- **Tiempo de Migración**: 1-2 semanas para implementación completa
- **Testing**: Tests existentes requieren actualización

### Riesgos

- **Dependencia Externa**: Service disruption si Supabase tiene issues
  - **Mitigación**: Circuit breaker y fallback plan definido
- **Curva de Aprendizaje**: Equipo necesita entender Supabase Auth
  - **Mitigación**: Training session y documentación actualizada
- **Migración Compleja**: Riesgo de downtime o bugs
  - **Mitigación**: Tests exhaustivos y gradual rollout

### Métricas de Éxito

- Todos los tests pasan con nueva implementación
- JWT validation funciona correctamente contra Supabase
- Login/Register operations funcionan sin issues
- Code coverage >80% para auth flows

## Implementación

### Fases

1. **Fase 1**: Instalar Supabase SDK + ADR - **Completado**
2. **Fase 2**: Program.cs con Authority + Audience - **En Progreso**
3. **Fase 3**: SupabaseAuthService (reemplaza custom JWT) - **Pendiente**
4. **Fase 4**: Update Login/Register use cases - **Pendiente**
5. **Fase 5**: Testing + Cleanup - **Pendiente**

### Dependencias

- **Supabase SDK**: v1.0.3 instalado
- **JWT Bearer Authentication**: Configurado para OIDC
- **Application Settings**: Supabase URL y anon key configuradas

### Rollback Plan

Si la migración falla:
1. Revertir Program.cs changes
2. Restaurar AuthService original desde git
3. Update connection strings para custom auth
4. Tests de regression para confirmar funcionalidad

## Timeline

**Fecha de Inicio**: 2025-09-28
**Fecha de Completación Estimada**: 2025-10-05
**Fecha de Revisión**: 2025-12-28 (3 meses después)

## Stakeholders

- **Architect**: @architect - Aprobar arquitectura
- **Lead Developer**: @lead-dev - Supervisar implementación
- **DevOps**: @devops - Validar configuración de producción

## Referencias

- **Issue**: Auth migration requirement
- **Rules**: `.clinerules/13-supabase-jwt.md`
- **Supabase Documentation**: https://supabase.com/docs/guides/auth
- **Microsoft OIDC**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/oidc

---

**Aprobado por**: Development Team

**Fecha de Aprobación**: 2025-09-28

**Próxima Revisión**: 2025-12-28
