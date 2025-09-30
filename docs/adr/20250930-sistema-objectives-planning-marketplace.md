# ADR-003: Sistema de Objectives, Planning y Marketplace

**Fecha**: 2025-09-30

**Estado**: Propuesta

**Categoría**: ARCH

**Tags**: domain-model, planning, marketplace, training, multi-tenant, saas

## Contexto

SportPlanner necesita evolucionar de un sistema básico de gestión de usuarios y equipos hacia una plataforma completa de planificación deportiva que permita a entrenadores:

1. **Definir objetivos de entrenamiento** (ej: "Mejorar cambio de mano", "Tiro libre bajo presión")
2. **Crear planificaciones temporales** con distribución de objetivos en el tiempo
3. **Generar entrenamientos automáticos** basados en objetivos y disponibilidad
4. **Reutilizar ejercicios** que cubran múltiples objetivos
5. **Compartir contenido** (objetivos, ejercicios, planificaciones) entre usuarios vía marketplace

### Problema

El sistema actual solo maneja Users, Subscriptions y Teams, sin capacidad de:
- Estructurar qué se entrena (objetivos)
- Planificar cuándo se entrena (planificaciones temporales)
- Ejecutar entrenamientos (sesiones con ejercicios)
- Compartir conocimiento (marketplace de contenido)

La construcción manual de cada entrenamiento es time-consuming y no escala. Se necesita un sistema que:
- Genere entrenamientos automáticamente basados en planificación
- Permita importar contenido oficial del sistema (seed data por deporte)
- Facilite compartir contenido entre usuarios (marketplace)
- Diferencie contenido del sistema vs contenido del usuario

### Antecedentes

- Sistema actual: Clean Architecture con .NET 8 + Angular 20 + Supabase Auth
- ADR-001: Migración a Supabase JWT (backend)
- ADR-002: Sistema de suscripciones multi-tenant
- Deportes soportados: Basketball, Football, Handball (extensible)
- Modelo multi-tenant: Una Subscription puede tener múltiples Teams

## Opciones Consideradas

### Opción 1: Modelo Simple (Todo User-Generated)

**Descripción**: Cada usuario crea todos sus objetivos/ejercicios desde cero. No hay contenido del sistema.

**Pros**:
- Implementación más rápida
- No requiere seed data masivo
- Modelo de datos más simple

**Contras**:
- Experiencia inicial pobre (usuario empieza de cero)
- No aprovecha conocimiento deportivo estándar
- Dificulta onboarding
- No hay efecto red (contenido compartido)

**Estimación de Esfuerzo**: Bajo

### Opción 2: Modelo Híbrido con System Content + Marketplace (ELEGIDA)

**Descripción**:
- Contenido oficial del sistema (seed data por deporte, read-only)
- Contenido del usuario (editable, privado por defecto)
- Marketplace para compartir contenido entre usuarios
- Filtrado por deporte en todas las vistas

**Pros**:
- Onboarding rápido (biblioteca oficial lista)
- Flexibilidad (usuario puede crear custom content)
- Efecto red (marketplace incentiva calidad)
- Escalabilidad (cache de system content)
- Separación clara de ownership

**Contras**:
- Requiere seed data inicial (esfuerzo de contenido)
- Modelo de datos más complejo (ownership, marketplace)
- Necesita estrategia de cache/paginación

**Estimación de Esfuerzo**: Alto

### Opción 3: Solo Marketplace (Sin System Content)

**Descripción**: Todo el contenido es user-generated, pero compartible vía marketplace.

**Pros**:
- No requiere seed data oficial
- Contenido evoluciona orgánicamente
- Menos mantenimiento de contenido oficial

**Contras**:
- Cold-start problem (primeros usuarios no tienen nada)
- Calidad inconsistente sin baseline oficial
- Dificulta estandarización por deporte

**Estimación de Esfuerzo**: Medio

## Requisitos No Funcionales Evaluados

- **Seguridad**: Multi-tenant isolation (SubscriptionId), contenido del sistema inmutable, validación de ownership
- **Rendimiento**: Cache de system content, paginación de listas grandes, índices por Sport
- **Mantenibilidad**: Clean Architecture, domain services para generación de workouts
- **Escalabilidad**: Seed data pre-cargado, lazy loading en UI, búsqueda indexada
- **Compatibilidad**: Se integra con Subscription/Team existentes, respeta arquitectura actual

## Decisión

### Opción Elegida: **Opción 2 - Modelo Híbrido con System Content + Marketplace**

**Justificación**:

1. **Onboarding crítico**: Usuarios necesitan ver valor inmediato. Contenido oficial por deporte acelera time-to-value.
2. **Escalabilidad**: System content se cachea, no crece con usuarios.
3. **Flexibilidad**: Usuarios pueden extender/customizar desde baseline oficial.
4. **Efecto red**: Marketplace incentiva creación de contenido de calidad.
5. **Diferenciación competitiva**: Biblioteca oficial por deporte es ventaja vs competidores.

## Consecuencias

### Impacto Positivo

- **Time-to-Value mejorado**: Usuarios arrancan con 200-300 objetivos oficiales por deporte
- **Reducción de esfuerzo manual**: Generación automática de entrenamientos basada en planificación
- **Efecto red**: Marketplace crea comunidad y contenido compartido
- **Escalabilidad**: System content se cachea, performance constante independiente de usuarios
- **Extensibilidad**: Agregar nuevos deportes = nuevo seed data, sin cambio arquitectónico

### Impacto Negativo

- **Esfuerzo de contenido inicial**: Requiere 200-300 objectives + exercises por deporte (seed data)
- **Complejidad de modelo**: Ownership enum, referencias nullable (SubscriptionId), lógica de clonación
- **Mantenimiento de system content**: Actualizaciones requieren migrations/versioning
- **Cache invalidation**: Cambios en system content requieren estrategia de propagación

### Riesgos

- **Riesgo 1: Calidad de system content** - [Media] - [Alto Impacto]
  - **Mitigación**: Revisión por expertos deportivos, versionado, feedback de usuarios

- **Riesgo 2: Performance en búsqueda con volumen alto** - [Baja] - [Medio Impacto]
  - **Mitigación**: Índices full-text, cache Redis, paginación agresiva, Elasticsearch futuro

- **Riesgo 3: Abuse del marketplace** - [Media] - [Medio Impacto]
  - **Mitigación**: Moderación manual inicial, reporting de contenido, rate limiting en publicaciones

### Métricas de Éxito

- **Adoption Rate**: >80% usuarios usan system objectives en sus primeras planificaciones
- **Content Creation**: >20% usuarios publican contenido en marketplace tras 3 meses
- **Generation Success**: >90% workouts generados automáticamente sin edición manual
- **Performance**: Búsqueda en marketplace <200ms p95 con 10K+ items
- **Engagement**: Avg 4+ stars en system content tras 6 meses

## Implementación

### Modelo de Dominio

#### Entidades Core

**Objective** (antes Goal)
```csharp
- Guid? SubscriptionId (NULL si System)
- ContentOwnership Ownership (User/System/MarketplaceUser)
- Sport Sport
- string Name
- string Description
- Guid ObjectiveCategoryId (FK)
- Guid? ObjectiveSubcategoryId (FK, nullable)
- List<ObjectiveTechnique> Techniques
- bool IsActive
- Guid? SourceMarketplaceItemId (si viene de marketplace)
```

**ObjectiveCategory** (Master Data)
```csharp
- string Name (Técnica Individual, Técnica Colectiva, Física, Táctica)
- Sport Sport
```

**ObjectiveSubcategory** (Master Data)
```csharp
- Guid ObjectiveCategoryId (FK)
- string Name (Ataque, Defensa, Transición)
```

**ObjectiveTechnique** (Child Entity)
```csharp
- Guid ObjectiveId (FK)
- string Description ("mantener vista alta", "cambio rápido")
- int Order
```

**TrainingPlan**
```csharp
- Guid SubscriptionId (NOT NULL - siempre usuario)
- string Name
- DateTime StartDate
- DateTime EndDate
- TrainingSchedule Schedule (Value Object)
- List<PlanObjective> Objectives (M:M)
- bool IsActive
- Guid? MarketplaceItemId
```

**TrainingSchedule** (Value Object)
```csharp
- DayOfWeek[] TrainingDays
- Dictionary<DayOfWeek, int> HoursPerDay
- int TotalWeeks { get; }
- int TotalSessions { get; }
- int TotalHours { get; }
```

**PlanObjective** (Junction con metadata)
```csharp
- Guid TrainingPlanId (FK)
- Guid ObjectiveId (FK - puede ser System o User)
- int Priority (1-5)
- int TargetSessions (distribución planificada)
```

**Workout** (Sesión de Entrenamiento)
```csharp
- Guid TrainingPlanId (FK)
- Guid SubscriptionId (FK)
- DateTime ScheduledDate
- int DurationMinutes
- string Name (auto-generado)
- List<WorkoutObjective> Objectives
- List<WorkoutExercise> Exercises
- WorkoutStatus Status (Planned/InProgress/Completed/Cancelled)
- DateTime? ActualStartTime
- DateTime? ActualEndTime
- string? Notes
- Guid? MarketplaceItemId
```

**Exercise** (Ejercicio Reutilizable)
```csharp
- Guid? SubscriptionId (NULL si System)
- ContentOwnership Ownership
- Sport Sport
- string Name
- string Description
- List<ExerciseObjective> Objectives (M:M)
- int DurationMinutes
- Difficulty Difficulty (Beginner/Intermediate/Advanced)
- int? Sets, Reps
- string? VideoUrl
- Guid? SourceMarketplaceItemId
```

**MarketplaceItem** (Tabla Polimórfica)
```csharp
- MarketplaceItemType Type (Objective/Exercise/Workout/Plan)
- Sport Sport (ÍNDICE PRINCIPAL)
- Guid? SourceEntityId (NULL si es System)
- ContentOwnership SourceOwnership
- Guid? PublishedBySubscriptionId (NULL si System)
- string Name
- string Description
- bool IsSystemOfficial
- decimal AverageRating (0-5)
- int TotalRatings
- int TotalDownloads
- int TotalViews
- DateTime PublishedAt
```

**MarketplaceRating**
```csharp
- Guid MarketplaceItemId (FK)
- Guid RatedBySubscriptionId (FK)
- int Stars (1-5)
- string? Comment
- DateTime CreatedAt
```

### Flujo de Contenido

```
┌─────────────────────────────────────────────────────────┐
│ SYSTEM CONTENT (Ownership=System)                       │
│ - SubscriptionId = NULL                                 │
│ - Pre-cargado en migrations (seed data)                 │
│ - Read-only para usuarios                               │
│ - Visible en Marketplace automáticamente                │
│ - Cache agresivo (Redis/Memory, 1h TTL)                 │
└─────────────────────────────────────────────────────────┘
                            ↓
                    Usuario ve biblioteca oficial
                            ↓
           ┌────────────────┴────────────────┐
           ↓                                  ↓
┌──────────────────────┐          ┌──────────────────────┐
│ USA DIRECTO          │          │ CLONA Y EDITA        │
│ (read-only)          │          │ (ownership=User)     │
└──────────────────────┘          └──────────────────────┘
           ↓                                  ↓
   Añade a TrainingPlan            Crea copia editable
           ↓                                  ↓
   Genera Workouts                 Puede publicar en Marketplace
           ↓                                  ↓
┌─────────────────────────────────────────────────────────┐
│ MARKETPLACE DOWNLOAD (Ownership=MarketplaceUser)        │
│ - SubscriptionId = target                               │
│ - SourceMarketplaceItemId apunta al original            │
│ - Read-only (copia)                                     │
│ - Usuario puede "clonar" para convertir en User         │
└─────────────────────────────────────────────────────────┘
```

### Generación Automática de Workouts

**Domain Service: WorkoutAutoGeneratorService**

```csharp
public class WorkoutAutoGeneratorService
{
    public List<Workout> GenerateAllWorkouts(TrainingPlan plan)
    {
        // 1. Calcular sesiones totales desde Schedule
        var totalSessions = plan.Schedule.TotalSessions;
        var trainingDates = CalculateTrainingDates(plan);

        // 2. Distribuir objectives por sesiones según priority
        var objectiveDistribution = DistributeObjectives(
            plan.Objectives,
            totalSessions
        );

        // 3. Para cada fecha, generar workout
        var workouts = new List<Workout>();
        for (int i = 0; i < trainingDates.Length; i++)
        {
            var date = trainingDates[i];
            var objectivesForSession = objectiveDistribution[i];

            // 4. Buscar exercises que cubran los objectives
            var exercises = FindBestExercises(objectivesForSession);

            // 5. Crear Workout
            var workout = new Workout(
                plan.Id,
                plan.SubscriptionId,
                date,
                plan.Schedule.HoursPerDay[date.DayOfWeek] * 60,
                GenerateWorkoutName(objectivesForSession),
                objectivesForSession,
                exercises
            );

            workouts.Add(workout);
        }

        return workouts;
    }

    private List<Exercise> FindBestExercises(List<Objective> objectives)
    {
        // Buscar exercises (System + User) que cubran máximo de objectives
        // Priorizar exercises que cubren múltiples objectives
        // Balancear dificultad
    }
}
```

### Búsqueda en Marketplace

**Query Optimizado:**
```csharp
public class SearchMarketplaceQuery : IRequest<PagedResult<MarketplaceItemDto>>
{
    public Sport Sport { get; set; } // REQUERIDO
    public MarketplaceItemType? Type { get; set; }
    public MarketplaceFilter Filter { get; set; } // Official/Community/Popular/TopRated
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

**Índices de Performance:**
```sql
-- Marketplace por deporte y tipo
CREATE INDEX idx_marketplace_sport_type_rating
ON marketplace_items(sport, type, average_rating DESC)
WHERE is_system_official = false;

-- System content por deporte
CREATE INDEX idx_objectives_sport_system
ON objectives(sport, ownership)
WHERE is_active = true;

-- Búsqueda full-text
CREATE INDEX idx_marketplace_search
ON marketplace_items
USING GIN(to_tsvector('english', name || ' ' || description));
```

### Fases de Desarrollo

#### **FASE 0: System Content Seed (Prerequisito)**
**Duración**: 3-5 días

- Definir estructura de contenido oficial por deporte
- Basketball: 200-250 objectives + 100-150 exercises
- Football: Similar estructura
- Handball: Similar estructura
- Crear migrations con INSERT masivos
- Script de validación de seed data

#### **FASE 1: Domain Layer - Objectives**
**Duración**: 2-3 días

**Entidades:**
- Objective, ObjectiveCategory, ObjectiveSubcategory, ObjectiveTechnique
- ContentOwnership enum

**Use Cases:**
- CreateObjectiveCommand/Handler
- UpdateObjectiveCommand/Handler
- CloneObjectiveCommand/Handler (System → User)
- GetObjectivesBySubscriptionQuery/Handler
- GetSystemObjectivesBySportQuery/Handler (con cache)

**Tests:**
- Domain: Validaciones de Objective
- Application: Command handlers con mock repositories
- Integration: CRUD completo con DB

#### **FASE 2: Domain Layer - Training Plans**
**Duración**: 3-4 días

**Entidades:**
- TrainingPlan, TrainingSchedule (Value Object), PlanObjective

**Use Cases:**
- CreateTrainingPlanCommand/Handler
- AddObjectiveToPlanCommand/Handler
- UpdateObjectivePriorityCommand/Handler
- GetPlansBySubscriptionQuery/Handler

**Domain Services:**
- WorkoutGeneratorService (lógica de distribución)

**Tests:**
- TrainingSchedule calculations (TotalSessions, TotalHours)
- Distribution logic (priority-based)

#### **FASE 3: Domain Layer - Workouts & Exercises**
**Duración**: 4-5 días

**Entidades:**
- Workout, WorkoutObjective, WorkoutExercise
- Exercise, ExerciseObjective

**Use Cases:**
- GenerateWorkoutsFromPlanCommand/Handler (usa WorkoutGeneratorService)
- CreateExerciseCommand/Handler
- StartWorkoutCommand/Handler
- CompleteWorkoutCommand/Handler
- GetUpcomingWorkoutsQuery/Handler

**Tests:**
- Generación automática end-to-end
- Búsqueda de exercises por objectives

#### **FASE 4: Marketplace Backend**
**Duración**: 3-4 días

**Entidades:**
- MarketplaceItem, MarketplaceRating, MarketplaceDownload

**Use Cases:**
- PublishToMarketplaceCommand<T>/Handler
- SearchMarketplaceQuery/Handler (con filtros por Sport)
- DownloadFromMarketplaceCommand<T>/Handler (con lógica de clonación)
- RateMarketplaceItemCommand/Handler

**Tests:**
- Flujo completo: Publish → Search → Download → Rate
- Validación de Sport filtering
- Clone logic (dependencies en cascade)

#### **FASE 5: Infrastructure Layer**
**Duración**: 2-3 días

**Migrations:**
1. AddObjectiveEntities
2. AddTrainingPlanEntities
3. AddWorkoutAndExerciseEntities
4. AddMarketplaceEntities
5. SeedSystemContentBasketball
6. SeedSystemContentFootball
7. SeedSystemContentHandball

**Repositories:**
- ObjectiveRepository, TrainingPlanRepository, WorkoutRepository, ExerciseRepository, MarketplaceRepository
- Cache layer para System content (IDistributedCache)

**Configurations (EF Core):**
- Entity configurations para todas las nuevas entidades
- Índices de performance

#### **FASE 6: API Controllers**
**Duración**: 2-3 días

**Controllers:**
- ObjectivesController (CRUD + clone)
- TrainingPlansController (CRUD + generate workouts)
- WorkoutsController (list, update, start, complete)
- ExercisesController (CRUD + search by objective)
- MarketplaceController (search, download, rate)

**Swagger Documentation:**
- Todos los endpoints documentados
- Ejemplos de request/response

#### **FASE 7: Frontend - Objectives & Plans**
**Duración**: 4-5 días

**Features:**
- goals/ → objectives/ (rename)
  - objectives-list.component (infinite scroll, filter by category)
  - objective-form.component (create/edit)
  - system-objectives-browser.component (explorar biblioteca oficial)
  - objective-clone-dialog.component
- training-plans/
  - plans-list.component
  - plan-form.component
  - plan-schedule-editor.component (visual schedule builder)
  - plan-objectives-selector.component (modal con search)

**Services:**
- ObjectivesService (con cache local de system objectives)
- TrainingPlansService

#### **FASE 8: Frontend - Workouts & Exercises**
**Duración**: 4-5 días

**Features:**
- workouts/
  - workouts-calendar.component (vista calendario con próximas sesiones)
  - workout-details.component
  - workout-execution.component (cronómetro, marcar completados)
- exercises/
  - exercises-library.component (grid con filtros)
  - exercise-form.component
  - exercise-card.component (con video preview)

**Services:**
- WorkoutsService
- ExercisesService

#### **FASE 9: Frontend - Marketplace**
**Duración**: 3-4 días

**Features:**
- marketplace/
  - marketplace-browse.component (infinite scroll, filtros)
  - marketplace-item-details.component
  - marketplace-rating-dialog.component
  - my-published-items.component

**Components:**
- rating-stars.component (reutilizable)
- download-button.component (con feedback visual)

**Services:**
- MarketplaceService

#### **FASE 10: Testing & Polish**
**Duración**: 3-4 días

- Integration tests completos (Playwright)
- Coverage validation (>80% Application/Domain)
- Performance testing (búsqueda marketplace con 10K items)
- UX refinement (loading states, error handling)
- Accessibility audit (WCAG 2.1 AA)

### Dependencias

- **ADR-002 Subscriptions**: Sistema de objectives se vincula a Subscription
- **Supabase Auth**: JWT validation para ownership checks
- **EF Core Migrations**: Seed data masivo requiere migrations optimizadas
- **Angular Material + Tailwind**: UI components para selectors/modals

### Rollback Plan

**Si falla Fase 1-2 (Objectives/Plans)**:
- Revert migrations
- No impacta sistema existente (features aisladas)

**Si falla Fase 3-4 (Workouts/Marketplace)**:
- Sistema Objectives/Plans sigue funcional
- Workouts pueden crearse manualmente (sin generación automática)
- Marketplace puede postponerse sin impactar core features

**Si falla seed data (System Content)**:
- Sistema funciona 100% como user-generated content
- Seed data puede agregarse post-lanzamiento

## Timeline

**Fecha de Inicio**: 2025-10-01

**Fecha de Completación Estimada**: 2025-11-15 (7 semanas)

**Hitos:**
- Semana 1: FASE 0 (Seed data) + FASE 1 (Objectives)
- Semana 2: FASE 2 (Plans) + inicio FASE 3
- Semana 3: FASE 3 (Workouts/Exercises) + FASE 4 (Marketplace)
- Semana 4: FASE 5 (Infrastructure) + FASE 6 (API)
- Semana 5-6: FASE 7-9 (Frontend completo)
- Semana 7: FASE 10 (Testing/Polish)

**Fecha de Revisión**: 2025-12-15 (1 mes post-lanzamiento)

## Stakeholders

- **Architect**: Claude + Development Team
- **Product Owner**: Define contenido oficial (seed data)
- **Frontend Lead**: Angular 20 implementation
- **Backend Lead**: .NET 8 Clean Architecture
- **Sports Experts**: Validación de contenido oficial por deporte

## Referencias

- **CLAUDE.md**: Project architecture guidelines
- **.clinerules/**: Development standards
- **ADR-001**: Supabase JWT migration
- **ADR-002**: Sistema de suscripciones
- **Clean Architecture**: Jason Taylor template
- **DDD Patterns**: Aggregates, Value Objects, Domain Services

## Anexos

### Enums

```csharp
public enum ContentOwnership
{
    User,            // Creado por usuario (editable)
    System,          // Oficial de la plataforma (read-only)
    MarketplaceUser  // Descargado de otro usuario (read-only, clonable)
}

public enum MarketplaceItemType
{
    Objective,
    Exercise,
    Workout,
    TrainingPlan
}

public enum MarketplaceFilter
{
    All,
    OfficialOnly,
    CommunityOnly,
    Popular,
    TopRated
}

public enum WorkoutStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled
}

public enum Difficulty
{
    Beginner,
    Intermediate,
    Advanced
}
```

### Database Schema Summary

**Total nuevas tablas**: 14

**Core:**
- objectives, objective_categories, objective_subcategories, objective_techniques (4)
- training_plans, plan_objectives (2)
- workouts, workout_objectives, workout_exercises (3)
- exercises, exercise_objectives (2)

**Marketplace:**
- marketplace_items, marketplace_ratings, marketplace_downloads (3)

**Estimación de filas iniciales**:
- objectives (System): ~750 (250 por deporte × 3 deportes)
- exercises (System): ~450 (150 por deporte × 3 deportes)
- objective_categories: ~12-15
- objective_subcategories: ~20-25

---

**Aprobado por**: Pendiente

**Fecha de Aprobación**: Pendiente

**Próxima Revisión**: 2025-12-15