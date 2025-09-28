# ADR-003: Sistema de Suscripciones Backend

**Fecha**: 2025-09-29

**Estado**: Aceptada

**Categoría**: ARCH

**Tags**: subscription, billing, authorization, domain, supabase

## Contexto

SportPlanner requiere un sistema de suscripciones para monetizar el servicio, con diferentes niveles de acceso (Free, Team, Coach, Club) y límites de usuarios/equipos. Los usuarios Supabase serán propietarios de suscripciones las cuales pueden proporcionar acceso a otros usuarios (entrenadores).

### Problema

Actualmente no existe modelo de monetización ni control de recursos por usuario/pago. Todos los usuarios tendrían acceso ilimitado, lo cual es insostenible para un producto SaaS.

**Datos del problema**:
- 0% de ingresos actual
- Sin segmentación de usuarios por capacidad de pago
- Riesgo de abuso de recursos (usuarios, equipos)
- Falta de diferenciación feature por precio

### Antecedentes

Inicialmente SportPlanner se diseñó como aplicación gratuita con funcionalidades básicas. Con el crecimiento del proyecto, es necesario implementar monetización para:
- Mantener desarrollo sostenible
- Proporcionar valor superior para usuarios pagadores
- Controlar escalabilidad de infraestructura

## Opciones Consideradas

### Opción 1: Sistema de Suscripciones Integrado (Elegida)

**Descripción**: Crear modelo completo en backend con entidades Subscription, tipos y límites, integrada con identidad Supabase.

**Pros**:
- Control total sobre límites y billing
- Integración nativa con identity
- Escalabilidad por tenant
- Auditoría completa de acceso

**Contras**:
- Complejidad inicial de desarrollo alta
- Necesidad de lógica adicional en frontend
- Migración de usuarios existentes requerida
- Integración con payment gateways pending

**Estimación de Esfuerzo**: Alto (2-3 sprints)

### Opción 2: External SaaS (Stripe/Chargebee)

**Descripción**: Usar servicio externo de suscripciones con API integration, manejando solo autorización.

**Pros**:
- Desarrollo más rápido
- Gestión automática de billing/pagos
- Compliance integrada (PCI, etc.)
- Features avanzadas (trials, upgrades)

**Contras**:
- Dependencia externa
- Complejidad de sincronización
- Costos adicionales por volumen
- Menor control sobre business rules

**Estimación de Esfuerzo**: Medio

### Opción 3: Suscripción por Feature Flags

**Descripción**: Implementar como feature flags activados/desactivados por admin sin modelo de suscripción completo.

**Pros**:
- Implementación más simple inicialmente
- Menor cambio en base de datos
- Más control manual

**Contras**:
- Difícil monetización
- Administración manual escalada
- Sin self-service para usuarios
- Hardcoded en lugar de data-driven

**Estimación de Esfuerzo**: Bajo

## Requisitos No Funcionales Evaluados

- **Seguridad**: Necesario autorización por suscripción, integración con JWT Supabase
- **Rendimiento**: Checks de límites deben ser eficientes para no impactar UX
- **Mantenibilidad**: Modelo debe ser extensible para añadir nuevos tipos/precios
- **Escalabilidad**: Soporte multi-tenant por suscripción
- **Compatibilidad**: Buena integración con identidad existente y futura capacidad de pagos

## Decisión

### Opción Elegida: **Opción 1 - Sistema de Suscripciones Integrado**

Seleccionada porque proporciona el control empresarial necesario para monetizar SportPlanner manteniendo la arquitectura DDD. La integración completa permite logic business rules sofisticadas y reporting mejor, mientras que usar Supabase Auth mantiene consistencia con la infrastructure existente.

Justificación prioritaria:
- **Independencia**: Sin vendor lock-in en servicios costly
- **Business Logic**: Control sobre límites y reglas específicas de SportPlanner
- **Arquitectura**: Align con Clean Architecture y DDD principles
- **Crecimiento**: Fundamenta para features avanzadas (teams, analytics, etc.)

## Consecuencias

### Impacto Positivo

- **Monetización**: Base para generar ingresos sostenibles
- **Arquitectura**: Mejora separación de responsabilidades con subscription domain
- **User Experience**: Posibilita diferenciación de valor por usuario
- **Escalabilidad**: Control granular sobre resource usage

### Impacto Negativo

- **Complejidad**: Aumenta significativamente lógica de negocio
- **Desarrollo**: Requiere fase dedicada de implementation + testing
- **Migración**: Necesario migrar usuarios existentes a Free tier
- **Payment Integration**: Pendiente integración con gateways (Stripe)

### Riesgos

- **Adopción**: Usuarios Free podrían rechazar upgrade
  - **Mitigación**: Clear value proposition, generous trials
- **Complejidad dev**: Riesgo de bugs en authorization
  - **Mitigación**: TDD completo, pair programming para business logic
- **Performance**: Checks de límites podrían impactar response times
  - **Mitigación**: Caching de subscription info, optimización queries

### Métricas de Éxito

- **Coverage**: Tests unitarios >80% en domain logic
- **Performance**: Authorization checks <100ms en 95th percentile
- **User Growth**: Mantenimiento de retention rate post-suscripción
- **Technical**: Zero regression bugs en existing auth flows

## Implementación

### Modelo de Datos

```
Subscription
├── OwnerId (UUID Supabase)
├── Type (Free|Team|Coach|Club)
├── Sport (Football|Basketball|Handball - requerido)
├── MaxUsers (int según type)
├── MaxTeams (int según type)
├── IsActive (bool)
├── CreatedAt/UpdatedAt (IAuditable)

SubscriptionUser
├── SubscriptionId (FK)
├── UserId (UUID Supabase)
├── RoleInSubscription (Athlete|Coach|etc.)
├── GrantedBy (UUID)
├── GrantedAt (datetime)
```

### Fases de Implementación

1. **Fase 1**: Domain Entities + Business Rules (TDD) - Completar esta semana
2. **Fase 2**: EF Integration + Repositories - Semana siguiente
3. **Fase 3**: Application Layer (CQRS Use Cases) - Fase siguiente
4. **Fase 4**: API Controllers + Authorization - Finalizar mes
5. **Fase 5**: Migration Scripts + Testing completo - Próximo sprint
6. **Fase 6**: Payment Gateway Integration (Stripe) - Sprint después

### Dependencias

- **Supabase Auth Migration**: ADR-002 completado
- **Dotnet C#**: Framework .NET 8 listo
- **MediatR**: CQRS pattern establecido
- **EF Core**: ORM principles definidos

### Rollback Plan

Si necesidad de revertir:
1. Revertir DbMigrations (EF rollback)
2. Remover Subscription controllers/middleware
3. Restaurar auth guards previos
4. Desactivar subscription features en frontend

## Timeline

**Fecha de Inicio**: 2025-09-29
**Fecha de Completación Estimada**: 2025-10-31
**Fecha de Revisión**: 2025-12-29 (3 meses después - verificar métricas de éxito)

## Stakeholders

- **Architect**: jorgeturrubia - Aprobar architecture decisions
- **Lead Developer**: jorgeturrubia - Supervisar desarrollo
- **Product Owner**: - Validar límites de suscripción y UX flow

## Referencias

- **Issue**: Nueva tarea sistema suscripciones
- **Proyecto**: SportPlannerV2 backend
- **ADR Related**: ADR-002 (Supabase Auth Migration)
- **Rules Compliance**:
  - `.clinerules/01-clean-code.md` - SOLID, DRY
  - `.clinerules/14-dotnet-backend.md` - Clean Architecture
  - `.clinerules/04-testing.md` - TDD, 80%+ coverage
  - `.clinerules/13-supabase-jwt.md` - Auth integration

---

**Aprobado por**: jorgeturrubia

**Fecha de Aprobación**: 2025-09-29

**Próxima Revisión**: 2025-12-29
