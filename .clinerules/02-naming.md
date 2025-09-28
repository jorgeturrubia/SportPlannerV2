# Naming Conventions

## Archivos y Carpetas
- **Archivos**: kebab-case (ej: `user-profile.component.ts`)
- **Carpetas**: kebab-case (ej: `user-management/`)
- **Componentes Angular**: `feature-name.component.ts`
- **Servicios Angular**: `feature-name.service.ts`
- **Modelos**: `feature-name.model.ts`

## Clases y Interfaces
- **Componentes**: PascalCase + Component suffix (ej: `UserProfileComponent`)
- **Servicios**: PascalCase + Service suffix (ej: `UserManagementService`)
- **Interfaces**: PascalCase con I prefix opcional (ej: `User` o `IUser`)
- **Tipos**: PascalCase (ej: `UserRole`)
- **Enums**: PascalCase (ej: `UserStatus`)

## Variables y Funciones
- **Variables**: camelCase (ej: `userName`, `isAuthenticated`)
- **Funciones/Métodos**: camelCase con verbo inicial (ej: `getUserProfile`, `validateForm`)
- **Constantes**: SCREAMING_SNAKE_CASE (ej: `API_BASE_URL`, `MAX_RETRY_ATTEMPTS`)
- **Propiedades privadas**: prefijo underscore `_` (ej: `_internalState`)

## Específico del Dominio SportPlanner
- **Entidades**: `Training`, `Athlete`, `Coach`, `Session`, `Plan`
- **Servicios**: `TrainingService`, `AthleteService`, `PlanningService`
- **Estados**: `TrainingState`, `AthleteProgress`, `SessionStatus`
- **Eventos**: `TrainingCreated`, `SessionCompleted`, `PlanUpdated`

## Base de Datos
- **Tablas**: snake_case (ej: `training_sessions`, `athlete_profiles`)
- **Columnas**: snake_case (ej: `created_at`, `user_id`)
- **Índices**: formato descriptivo (ej: `idx_training_sessions_athlete_id`)

## URLs y Rutas
- **Rutas**: kebab-case (ej: `/training-plans`, `/athlete-profile`)
- **Query params**: camelCase (ej: `?athleteId=123&startDate=2024-01-01`)
- **API endpoints**: kebab-case con versión (ej: `/api/v1/training-plans`)
