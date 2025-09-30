# SportPlanner - Sistema de Objectives, Planning y Marketplace
## Especificación Técnica Completa

**Versión**: 1.0
**Fecha**: 2025-09-30
**Status**: Diseño Aprobado Pendiente

---

## 📋 Tabla de Contenidos

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Contexto y Motivación](#contexto-y-motivación)
3. [Arquitectura de Dominio](#arquitectura-de-dominio)
4. [Modelo de Datos Detallado](#modelo-de-datos-detallado)
5. [Flujos de Negocio](#flujos-de-negocio)
6. [Especificación de API](#especificación-de-api)
7. [Diseño de Frontend](#diseño-de-frontend)
8. [Estrategia de Testing](#estrategia-de-testing)
9. [Plan de Implementación por Fases](#plan-de-implementación-por-fases)
10. [Consideraciones de Performance](#consideraciones-de-performance)
11. [Seguridad y Multi-tenancy](#seguridad-y-multi-tenancy)
12. [Roadmap y Extensiones Futuras](#roadmap-y-extensiones-futuras)

---

## 1. Resumen Ejecutivo

### 1.1 Objetivo del Sistema

Construir un sistema completo de planificación deportiva SaaS que permita a entrenadores:

- ✅ **Definir objetivos de entrenamiento** estructurados por categorías deportivas
- ✅ **Crear planificaciones temporales** con distribución automática de objetivos
- ✅ **Generar entrenamientos automáticos** basados en disponibilidad y prioridades
- ✅ **Gestionar biblioteca de ejercicios** vinculados a múltiples objetivos
- ✅ **Compartir y descargar contenido** vía marketplace con sistema de valoraciones

### 1.2 Propuesta de Valor

| Stakeholder | Valor Entregado |
|-------------|-----------------|
| **Entrenadores** | Reducción 70% tiempo en planificación manual, acceso a biblioteca oficial de 750+ objetivos |
| **Clubes Deportivos** | Estandarización de metodología, sharing de contenido entre equipos |
| **Plataforma** | Efecto red vía marketplace, contenido de calidad generado por comunidad |

### 1.3 Tecnologías Core

- **Backend**: .NET 8, Clean Architecture, Entity Framework Core, PostgreSQL
- **Frontend**: Angular 20 (standalone), Material Design, Tailwind CSS v4
- **Auth**: Supabase JWT con JWKS validation
- **Caching**: Redis/IDistributedCache para system content
- **Search**: PostgreSQL full-text search (migración futura a Elasticsearch)

---

## 2. Contexto y Motivación

### 2.1 Problema Actual

**Pain Points Identificados:**

1. **Creación manual time-consuming**: Entrenadores pasan 2-3 horas/semana diseñando sesiones desde cero
2. **Falta de estructura**: Objetivos no formalizados, seguimiento inconsistente
3. **Reinvención constante**: Cada entrenador crea su propia "base de conocimiento"
4. **Zero collaboration**: No hay forma de compartir mejores prácticas entre clubes/entrenadores
5. **Onboarding lento**: Entrenadores nuevos empiezan sin biblioteca de referencia

### 2.2 Solución Propuesta

**Sistema de 4 Capas:**

```
┌──────────────────────────────────────────────────────────────┐
│ CAPA 1: OBJECTIVES (Qué entrenar)                            │
│ - Biblioteca oficial: 750+ objetivos por deporte             │
│ - Clasificación: Categoría → Subcategoría → Técnicas         │
│ - User content: Crear objetivos custom editables             │
└──────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────────┐
│ CAPA 2: TRAINING PLANS (Cuándo entrenar)                     │
│ - Planificación temporal: Fecha inicio/fin                   │
│ - Schedule: Días/semana + horas/día                          │
│ - Asignación: Objetivos con prioridades                      │
└──────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────────┐
│ CAPA 3: WORKOUTS + EXERCISES (Cómo entrenar)                 │
│ - Generación automática de sesiones                          │
│ - Biblioteca de ejercicios vinculados a objetivos            │
│ - Ejecución: Timer, tracking, notas post-sesión              │
└──────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────────┐
│ CAPA 4: MARKETPLACE (Compartir conocimiento)                 │
│ - Publicación de contenido (objectives/exercises/plans)      │
│ - Sistema de ratings (0-5 estrellas + reviews)               │
│ - Descarga e importación con clonación                       │
└──────────────────────────────────────────────────────────────┘
```

### 2.3 Diferenciadores Competitivos

| Feature | Competidor A | Competidor B | SportPlanner |
|---------|--------------|--------------|--------------|
| Biblioteca oficial por deporte | ❌ | ✅ (limitada) | ✅ (750+) |
| Generación automática de workouts | ❌ | ❌ | ✅ |
| Marketplace de contenido | ❌ | ❌ | ✅ |
| Multi-tenant SaaS | ✅ | ❌ | ✅ |
| Contenido mixto System+User | ❌ | ❌ | ✅ |

---

## 3. Arquitectura de Dominio

### 3.1 Domain Model Completo

```
┌────────────────────────────────────────────────────────────────┐
│                         SUBSCRIPTION                            │
│                       (Multi-tenant Root)                       │
│  - Sport: Basketball/Football/Handball                         │
│  - MaxUsers, MaxTeams (subscription tiers)                     │
└────────────┬───────────────────────────────────────────────────┘
             │
             ├─────────────────────────────────────────────┐
             ↓                                             ↓
┌──────────────────────┐                     ┌──────────────────────┐
│   OBJECTIVE          │                     │   EXERCISE           │
│   (Aggregate Root)   │                     │   (Aggregate Root)   │
├──────────────────────┤                     ├──────────────────────┤
│ ? SubscriptionId     │←─────────────┐     │ ? SubscriptionId     │
│ Ownership            │              │     │ Ownership            │
│ Sport                │              │     │ Sport                │
│ Name                 │              │     │ Name                 │
│ Description          │              │     │ DurationMinutes      │
│ CategoryId (FK)      │              │     │ Difficulty           │
│ ? SubcategoryId (FK) │              │     │ Sets, Reps           │
│ IsActive             │              │     │ VideoUrl             │
│ ? MarketplaceItemId  │              │     │ ? MarketplaceItemId  │
└────────┬─────────────┘              │     └──────────┬───────────┘
         │                            │                │
         ↓                            │                ↓
┌──────────────────────┐              │     ┌──────────────────────┐
│ OBJECTIVE_TECHNIQUE  │              │     │ EXERCISE_OBJECTIVE   │
│ (Child Collection)   │              │     │ (Junction M:M)       │
├──────────────────────┤              │     ├──────────────────────┤
│ ObjectiveId (FK)     │              │     │ ExerciseId (FK)      │
│ Description          │              └─────┤ ObjectiveId (FK)     │
│ Order                │                    └──────────────────────┘
└──────────────────────┘
             ↑                                        ↑
             └────────────────┬───────────────────────┘
                              │
                ┌─────────────┴────────────────┐
                ↓                              ↓
┌──────────────────────────┐      ┌──────────────────────────┐
│   TRAINING_PLAN          │      │   WORKOUT                │
│   (Aggregate Root)       │      │   (Aggregate Root)       │
├──────────────────────────┤      ├──────────────────────────┤
│ SubscriptionId (FK)      │      │ TrainingPlanId (FK)      │
│ Name                     │      │ SubscriptionId (FK)      │
│ StartDate, EndDate       │      │ ScheduledDate            │
│ Schedule (VO)            │      │ DurationMinutes          │
│ IsActive                 │      │ Status (enum)            │
│ ? MarketplaceItemId      │      │ ActualStartTime          │
└────────┬─────────────────┘      │ ActualEndTime            │
         │                        │ Notes                    │
         ↓                        │ ? MarketplaceItemId      │
┌──────────────────────┐          └──────┬───────────────────┘
│ PLAN_OBJECTIVE       │                 │
│ (Junction + Meta)    │                 ↓
├──────────────────────┤          ┌──────────────────────┐
│ TrainingPlanId (FK)  │          │ WORKOUT_OBJECTIVE    │
│ ObjectiveId (FK)     │          │ (Junction + Meta)    │
│ Priority (1-5)       │          ├──────────────────────┤
│ TargetSessions       │          │ WorkoutId (FK)       │
└──────────────────────┘          │ ObjectiveId (FK)     │
                                  │ TimeAllocatedMinutes │
                                  └──────────────────────┘

                                  ┌──────────────────────┐
                                  │ WORKOUT_EXERCISE     │
                                  │ (Junction + Order)   │
                                  ├──────────────────────┤
                                  │ WorkoutId (FK)       │
                                  │ ExerciseId (FK)      │
                                  │ Order (int)          │
                                  │ DurationMinutes      │
                                  └──────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│                    MARKETPLACE_ITEM                            │
│                    (Polymorphic Index)                         │
├────────────────────────────────────────────────────────────────┤
│ Type (Objective/Exercise/Workout/Plan)                         │
│ Sport (ÍNDICE PRINCIPAL - filtro obligatorio)                  │
│ ? SourceEntityId (FK polimórfica)                              │
│ SourceOwnership (User/System)                                  │
│ ? PublishedBySubscriptionId                                    │
│ Name, Description                                              │
│ IsSystemOfficial (badge)                                       │
│ AverageRating (0-5), TotalRatings, TotalDownloads, TotalViews  │
└────────────┬───────────────────────────────────────────────────┘
             │
             ├──────────────────────────┐
             ↓                          ↓
┌──────────────────────┐    ┌──────────────────────────┐
│ MARKETPLACE_RATING   │    │ MARKETPLACE_DOWNLOAD     │
├──────────────────────┤    ├──────────────────────────┤
│ MarketplaceItemId FK │    │ MarketplaceItemId (FK)   │
│ RatedBySubscriptionId│    │ DownloadedBySubscriptionId│
│ Stars (1-5)          │    │ DownloadedAt             │
│ Comment              │    └──────────────────────────┘
│ CreatedAt            │
└──────────────────────┘

MASTER DATA TABLES:
┌──────────────────────┐    ┌──────────────────────────┐
│ OBJECTIVE_CATEGORY   │    │ OBJECTIVE_SUBCATEGORY    │
├──────────────────────┤    ├──────────────────────────┤
│ Name                 │    │ ObjectiveCategoryId (FK) │
│ Sport                │    │ Name                     │
└──────────────────────┘    └──────────────────────────┘
```

### 3.2 Enums del Sistema

```csharp
// Ownership de contenido
public enum ContentOwnership
{
    User,            // Creado por usuario (totalmente editable)
    System,          // Oficial de plataforma (read-only, pre-cargado)
    MarketplaceUser  // Descargado de otro usuario (read-only, clonable)
}

// Deportes soportados (extensible)
public enum Sport
{
    Basketball = 1,
    Football = 2,
    Handball = 3
}

// Tipos de items en marketplace
public enum MarketplaceItemType
{
    Objective = 1,
    Exercise = 2,
    Workout = 3,
    TrainingPlan = 4
}

// Filtros de búsqueda en marketplace
public enum MarketplaceFilter
{
    All,              // Todo el contenido
    OfficialOnly,     // Solo System content
    CommunityOnly,    // Solo User content
    Popular,          // Ordenado por downloads DESC
    TopRated          // Ordenado por AverageRating DESC
}

// Estados de un workout
public enum WorkoutStatus
{
    Planned = 1,      // Generado pero no iniciado
    InProgress = 2,   // Entrenamiento en curso
    Completed = 3,    // Finalizado con éxito
    Cancelled = 4     // Cancelado (mal tiempo, lesión, etc.)
}

// Dificultad de ejercicios
public enum Difficulty
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3
}

// Categorías de objetivos (master data)
public enum ObjectiveCategoryType
{
    IndividualTechnique = 1,  // Técnica Individual
    CollectiveTechnique = 2,  // Técnica Colectiva
    Physical = 3,             // Preparación Física
    Tactical = 4              // Táctica
}

// Subcategorías (master data)
public enum ObjectiveSubcategoryType
{
    Attack = 1,       // Ataque
    Defense = 2,      // Defensa
    Transition = 3,   // Transición
    SetPiece = 4      // Jugadas ensayadas
}
```

### 3.3 Value Objects

#### TrainingSchedule

```csharp
public class TrainingSchedule : ValueObject
{
    public DayOfWeek[] TrainingDays { get; private set; }
    public Dictionary<DayOfWeek, int> HoursPerDay { get; private set; }

    // Calculated properties
    public int TotalWeeks { get; private set; }
    public int TotalSessions { get; private set; }
    public int TotalHours { get; private set; }

    private TrainingSchedule() { } // EF Core

    public TrainingSchedule(
        DayOfWeek[] trainingDays,
        Dictionary<DayOfWeek, int> hoursPerDay,
        DateTime startDate,
        DateTime endDate)
    {
        if (trainingDays == null || trainingDays.Length == 0)
            throw new ArgumentException("At least one training day required");

        if (hoursPerDay == null || !trainingDays.All(d => hoursPerDay.ContainsKey(d)))
            throw new ArgumentException("Hours must be specified for all training days");

        TrainingDays = trainingDays;
        HoursPerDay = hoursPerDay;

        // Calculate totals
        var totalDays = (endDate - startDate).Days;
        TotalWeeks = (int)Math.Ceiling(totalDays / 7.0);
        TotalSessions = CalculateTotalSessions(startDate, endDate);
        TotalHours = HoursPerDay.Values.Sum() * (TotalSessions / TrainingDays.Length);
    }

    private int CalculateTotalSessions(DateTime start, DateTime end)
    {
        int count = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (TrainingDays.Contains(date.DayOfWeek))
                count++;
        }
        return count;
    }

    public DateTime[] GetAllTrainingDates(DateTime start, DateTime end)
    {
        var dates = new List<DateTime>();
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (TrainingDays.Contains(date.DayOfWeek))
                dates.Add(date);
        }
        return dates.ToArray();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return string.Join(",", TrainingDays.OrderBy(d => d));
        foreach (var kvp in HoursPerDay.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}
```

---

## 4. Modelo de Datos Detallado

### 4.1 Schema SQL Completo

```sql
-- ================================================================
-- OBJECTIVES (Objetivos de Entrenamiento)
-- ================================================================

-- Master Data: Categorías de objetivos por deporte
CREATE TABLE objective_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    sport INTEGER NOT NULL, -- Sport enum
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    UNIQUE(name, sport)
);

-- Master Data: Subcategorías (ej: Ataque, Defensa)
CREATE TABLE objective_subcategories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    objective_category_id UUID NOT NULL REFERENCES objective_categories(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    UNIQUE(objective_category_id, name)
);

-- Tabla principal de objetivos (hybrid: System + User content)
CREATE TABLE objectives (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subscription_id UUID REFERENCES subscriptions(id) ON DELETE CASCADE, -- NULL for System content
    ownership INTEGER NOT NULL, -- ContentOwnership enum
    sport INTEGER NOT NULL, -- Sport enum
    name VARCHAR(200) NOT NULL,
    description TEXT,
    objective_category_id UUID NOT NULL REFERENCES objective_categories(id),
    objective_subcategory_id UUID REFERENCES objective_subcategories(id), -- nullable
    is_active BOOLEAN NOT NULL DEFAULT true,
    source_marketplace_item_id UUID REFERENCES marketplace_items(id), -- si viene de marketplace

    -- Auditoría
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    created_by VARCHAR(255) NOT NULL,
    updated_at TIMESTAMP,
    updated_by VARCHAR(255),

    -- Constraints
    CHECK (ownership = 1 OR subscription_id IS NOT NULL), -- System content tiene subscription_id NULL
    UNIQUE(subscription_id, name) -- unique por suscripción (permite duplicados entre users)
);

-- Técnicas asociadas a objetivos (1:N)
CREATE TABLE objective_techniques (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    objective_id UUID NOT NULL REFERENCES objectives(id) ON DELETE CASCADE,
    description VARCHAR(500) NOT NULL,
    display_order INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT now()
);

-- Índices de performance
CREATE INDEX idx_objectives_subscription ON objectives(subscription_id) WHERE is_active = true;
CREATE INDEX idx_objectives_sport_ownership ON objectives(sport, ownership) WHERE is_active = true;
CREATE INDEX idx_objectives_category ON objectives(objective_category_id);
CREATE INDEX idx_objectives_search ON objectives USING GIN(to_tsvector('english', name || ' ' || description));

-- ================================================================
-- TRAINING PLANS (Planificaciones)
-- ================================================================

CREATE TABLE training_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,

    -- TrainingSchedule (serialized as JSON for simplicity, or normalized)
    training_days INTEGER[] NOT NULL, -- Array of DayOfWeek enum values
    hours_per_day JSONB NOT NULL, -- {"Monday": 2, "Wednesday": 1.5, "Friday": 2}

    is_active BOOLEAN NOT NULL DEFAULT true,
    marketplace_item_id UUID REFERENCES marketplace_items(id),

    -- Auditoría
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    created_by VARCHAR(255) NOT NULL,
    updated_at TIMESTAMP,
    updated_by VARCHAR(255),

    CHECK (end_date > start_date),
    UNIQUE(subscription_id, name)
);

-- Junction table: Plans <-> Objectives (M:M con metadata)
CREATE TABLE plan_objectives (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    training_plan_id UUID NOT NULL REFERENCES training_plans(id) ON DELETE CASCADE,
    objective_id UUID NOT NULL REFERENCES objectives(id) ON DELETE CASCADE,
    priority INTEGER NOT NULL DEFAULT 3, -- 1 (lowest) to 5 (highest)
    target_sessions INTEGER NOT NULL, -- Cuántas sesiones dedicar a este objetivo
    created_at TIMESTAMP NOT NULL DEFAULT now(),

    UNIQUE(training_plan_id, objective_id)
);

CREATE INDEX idx_training_plans_subscription ON training_plans(subscription_id) WHERE is_active = true;
CREATE INDEX idx_plan_objectives_plan ON plan_objectives(training_plan_id);
CREATE INDEX idx_plan_objectives_objective ON plan_objectives(objective_id);

-- ================================================================
-- EXERCISES (Ejercicios Reutilizables)
-- ================================================================

CREATE TABLE exercises (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subscription_id UUID REFERENCES subscriptions(id) ON DELETE CASCADE, -- NULL for System content
    ownership INTEGER NOT NULL, -- ContentOwnership enum
    sport INTEGER NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    duration_minutes INTEGER NOT NULL,
    difficulty INTEGER NOT NULL, -- Difficulty enum
    sets INTEGER,
    reps INTEGER,
    video_url VARCHAR(500),
    diagram_url VARCHAR(500),
    is_active BOOLEAN NOT NULL DEFAULT true,
    source_marketplace_item_id UUID REFERENCES marketplace_items(id),

    -- Auditoría
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    created_by VARCHAR(255) NOT NULL,
    updated_at TIMESTAMP,
    updated_by VARCHAR(255),

    CHECK (ownership = 1 OR subscription_id IS NOT NULL),
    UNIQUE(subscription_id, name)
);

-- Junction: Exercises <-> Objectives (M:M)
CREATE TABLE exercise_objectives (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exercise_id UUID NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
    objective_id UUID NOT NULL REFERENCES objectives(id) ON DELETE CASCADE,
    created_at TIMESTAMP NOT NULL DEFAULT now(),

    UNIQUE(exercise_id, objective_id)
);

CREATE INDEX idx_exercises_subscription ON exercises(subscription_id) WHERE is_active = true;
CREATE INDEX idx_exercises_sport_ownership ON exercises(sport, ownership) WHERE is_active = true;
CREATE INDEX idx_exercise_objectives_exercise ON exercise_objectives(exercise_id);
CREATE INDEX idx_exercise_objectives_objective ON exercise_objectives(objective_id);

-- ================================================================
-- WORKOUTS (Sesiones de Entrenamiento)
-- ================================================================

CREATE TABLE workouts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    training_plan_id UUID NOT NULL REFERENCES training_plans(id) ON DELETE CASCADE,
    subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
    scheduled_date DATE NOT NULL,
    duration_minutes INTEGER NOT NULL,
    name VARCHAR(200) NOT NULL,
    status INTEGER NOT NULL DEFAULT 1, -- WorkoutStatus enum

    -- Ejecución real
    actual_start_time TIMESTAMP,
    actual_end_time TIMESTAMP,
    notes TEXT, -- Observaciones post-sesión

    marketplace_item_id UUID REFERENCES marketplace_items(id),

    -- Auditoría
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    created_by VARCHAR(255) NOT NULL,
    updated_at TIMESTAMP,
    updated_by VARCHAR(255),

    UNIQUE(training_plan_id, scheduled_date)
);

-- Junction: Workouts <-> Objectives (con tiempo asignado)
CREATE TABLE workout_objectives (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    workout_id UUID NOT NULL REFERENCES workouts(id) ON DELETE CASCADE,
    objective_id UUID NOT NULL REFERENCES objectives(id) ON DELETE CASCADE,
    time_allocated_minutes INTEGER NOT NULL, -- Cuánto tiempo dedicar en esta sesión
    created_at TIMESTAMP NOT NULL DEFAULT now(),

    UNIQUE(workout_id, objective_id)
);

-- Junction: Workouts <-> Exercises (con orden de ejecución)
CREATE TABLE workout_exercises (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    workout_id UUID NOT NULL REFERENCES workouts(id) ON DELETE CASCADE,
    exercise_id UUID NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
    display_order INTEGER NOT NULL, -- Orden de ejecución en la sesión
    duration_minutes INTEGER NOT NULL, -- Puede override la duración del Exercise
    created_at TIMESTAMP NOT NULL DEFAULT now(),

    UNIQUE(workout_id, exercise_id)
);

CREATE INDEX idx_workouts_plan ON workouts(training_plan_id);
CREATE INDEX idx_workouts_subscription_date ON workouts(subscription_id, scheduled_date);
CREATE INDEX idx_workouts_status ON workouts(status, scheduled_date);
CREATE INDEX idx_workout_objectives_workout ON workout_objectives(workout_id);
CREATE INDEX idx_workout_exercises_workout ON workout_exercises(workout_id);

-- ================================================================
-- MARKETPLACE (Compartir Contenido)
-- ================================================================

-- Tabla polimórfica que indexa todo el contenido publicable
CREATE TABLE marketplace_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type INTEGER NOT NULL, -- MarketplaceItemType enum
    sport INTEGER NOT NULL, -- ÍNDICE PRINCIPAL - filtro obligatorio
    source_entity_id UUID NOT NULL, -- FK polimórfica a Objective/Exercise/Workout/Plan
    source_ownership INTEGER NOT NULL, -- ContentOwnership enum
    published_by_subscription_id UUID REFERENCES subscriptions(id), -- NULL si es System content

    -- Metadata
    name VARCHAR(200) NOT NULL,
    description TEXT,
    is_system_official BOOLEAN NOT NULL DEFAULT false, -- Badge "Contenido Oficial"

    -- Stats
    average_rating DECIMAL(3,2) DEFAULT 0.0, -- 0.00 - 5.00
    total_ratings INTEGER DEFAULT 0,
    total_downloads INTEGER DEFAULT 0,
    total_views INTEGER DEFAULT 0,

    published_at TIMESTAMP NOT NULL DEFAULT now(),
    updated_at TIMESTAMP,

    -- Soft delete (items descargados nunca se borran físicamente)
    is_active BOOLEAN NOT NULL DEFAULT true
);

-- Valoraciones de usuarios
CREATE TABLE marketplace_ratings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    marketplace_item_id UUID NOT NULL REFERENCES marketplace_items(id) ON DELETE CASCADE,
    rated_by_subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
    stars INTEGER NOT NULL CHECK (stars >= 1 AND stars <= 5),
    comment TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    updated_at TIMESTAMP,

    UNIQUE(marketplace_item_id, rated_by_subscription_id) -- Un usuario solo puede valorar una vez
);

-- Historial de descargas (para analytics)
CREATE TABLE marketplace_downloads (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    marketplace_item_id UUID NOT NULL REFERENCES marketplace_items(id) ON DELETE CASCADE,
    downloaded_by_subscription_id UUID NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
    downloaded_at TIMESTAMP NOT NULL DEFAULT now(),

    -- Permitir descargas múltiples del mismo item (para re-import)
    INDEX(marketplace_item_id, downloaded_by_subscription_id, downloaded_at)
);

-- Índices críticos de performance
CREATE INDEX idx_marketplace_sport_type ON marketplace_items(sport, type) WHERE is_active = true;
CREATE INDEX idx_marketplace_sport_rating ON marketplace_items(sport, average_rating DESC) WHERE is_active = true;
CREATE INDEX idx_marketplace_sport_downloads ON marketplace_items(sport, total_downloads DESC) WHERE is_active = true;
CREATE INDEX idx_marketplace_official ON marketplace_items(sport, is_system_official) WHERE is_active = true AND is_system_official = true;
CREATE INDEX idx_marketplace_search ON marketplace_items USING GIN(to_tsvector('english', name || ' ' || description));

CREATE INDEX idx_marketplace_ratings_item ON marketplace_ratings(marketplace_item_id);
CREATE INDEX idx_marketplace_downloads_item ON marketplace_downloads(marketplace_item_id);
```

### 4.2 Estimación de Volumen de Datos

**Seed Data Inicial (System Content):**

| Tabla | Filas | Comentario |
|-------|-------|------------|
| objective_categories | 12 | 4 categorías × 3 deportes |
| objective_subcategories | 24 | ~8 subcategorías × 3 deportes |
| objectives (System) | 750 | 250 por deporte × 3 |
| objective_techniques | 2250 | Avg 3 técnicas por objetivo |
| exercises (System) | 450 | 150 por deporte × 3 |
| exercise_objectives | 900 | Avg 2 objetivos por ejercicio |

**Proyección a 1 año (1000 usuarios activos):**

| Tabla | Filas Estimadas | Rationale |
|-------|-----------------|-----------|
| subscriptions | 1000 | 1 por usuario |
| objectives (User) | 5000 | Avg 5 custom por usuario |
| training_plans | 3000 | Avg 3 planes por usuario |
| workouts | 120000 | Avg 120 sesiones/año por usuario activo (3/semana × 40 semanas) |
| exercises (User) | 10000 | Avg 10 custom por usuario |
| marketplace_items | 2000 | 20% usuarios publican contenido |
| marketplace_ratings | 5000 | Avg 2.5 ratings por item publicado |

---

## 5. Flujos de Negocio

### 5.1 Flujo Completo: Creación de Planificación y Generación de Workouts

```
┌─────────────────────────────────────────────────────────────────┐
│ PASO 1: Usuario explora biblioteca de objetivos                │
├─────────────────────────────────────────────────────────────────┤
│ UI: /objectives/browse                                          │
│                                                                 │
│ Usuario ve:                                                     │
│ - 250 objetivos oficiales de Basketball (System)               │
│ - Clasificados por categorías/subcategorías                    │
│ - Buscador: "cambio de mano", "tiro libre", etc.               │
│                                                                 │
│ Acciones:                                                       │
│ [A] Usar objetivo System directamente (read-only)              │
│ [B] Clonar objetivo System para editar (→ User content)        │
│ [C] Crear objetivo desde cero (→ User content)                 │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 2: Crear Training Plan                                    │
├─────────────────────────────────────────────────────────────────┤
│ UI: /training-plans/create                                      │
│                                                                 │
│ Form:                                                           │
│ - Name: "Preparación Pretemporada 2025"                        │
│ - StartDate: 2025-08-01                                        │
│ - EndDate: 2025-09-30 (8 semanas)                              │
│ - TrainingDays: [Lunes, Miércoles, Viernes]                   │
│ - HoursPerDay: {Lunes: 2h, Miércoles: 1.5h, Viernes: 2h}      │
│                                                                 │
│ Cálculo automático:                                             │
│ → TotalSessions: 24 (3 días × 8 semanas)                       │
│ → TotalHours: 132h (24 sesiones × 5.5h avg)                    │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 3: Asignar Objetivos al Plan                              │
├─────────────────────────────────────────────────────────────────┤
│ UI: /training-plans/{id}/objectives                             │
│                                                                 │
│ Usuario selecciona (desde System + User objectives):           │
│ 1. "Cambio de mano rápido" (System) - Priority: 5 (High)       │
│ 2. "Tiro libre bajo presión" (System) - Priority: 4            │
│ 3. "Defensa 1v1" (System) - Priority: 3                        │
│ 4. "Transiciones rápidas" (User custom) - Priority: 4          │
│ 5. "Comunicación defensiva" (System) - Priority: 3             │
│                                                                 │
│ Sistema calcula distribución:                                   │
│ → Objetivo Priority 5: 30% sesiones = 7 sesiones               │
│ → Objetivo Priority 4: 25% sesiones = 6 sesiones cada uno      │
│ → Objetivo Priority 3: 20% sesiones = 5 sesiones cada uno      │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 4: Generar Workouts Automáticamente                       │
├─────────────────────────────────────────────────────────────────┤
│ API: POST /api/training-plans/{id}/generate-workouts           │
│                                                                 │
│ Domain Service: WorkoutAutoGeneratorService.GenerateAll()      │
│                                                                 │
│ Para cada fecha de entrenamiento (24 fechas):                  │
│   1. Seleccionar objetivos según distribución calculada        │
│   2. Para cada objetivo, buscar exercises (System + User):     │
│      - Filtrar por sport = Basketball                          │
│      - Priorizar exercises que cubran múltiples objetivos      │
│      - Balancear dificultad (Beginner → Advanced progresivo)   │
│   3. Crear Workout con:                                         │
│      - WorkoutObjectives (2-3 objectives por sesión)           │
│      - WorkoutExercises (5-8 exercises ordenados)              │
│      - DurationMinutes = suma de exercises ≈ 120min            │
│   4. Auto-generar Name: "Sesión 1: Cambio de mano + Defensa"  │
│                                                                 │
│ Resultado:                                                      │
│ → 24 workouts creados con Status=Planned                       │
│ → Usuario ve calendario completo poblado                       │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 5: Ejecutar Workout                                       │
├─────────────────────────────────────────────────────────────────┤
│ UI: /workouts/{id}/execute                                      │
│                                                                 │
│ Día del entrenamiento:                                          │
│ 1. Usuario abre workout del día                                │
│ 2. Revisa exercises planificados (puede modificar)             │
│ 3. Click "Iniciar Entrenamiento"                               │
│    → Status: Planned → InProgress                              │
│    → ActualStartTime = now()                                   │
│                                                                 │
│ Durante ejecución:                                              │
│ - Timer en pantalla (cuenta regresiva por exercise)            │
│ - Checklist: marcar exercises completados                      │
│ - Notas en tiempo real (ej: "Jugador X lesionado")            │
│                                                                 │
│ Al finalizar:                                                   │
│ 4. Click "Finalizar Entrenamiento"                             │
│    → Status: InProgress → Completed                            │
│    → ActualEndTime = now()                                     │
│    → Notes guardadas                                           │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 6: (Opcional) Publicar en Marketplace                     │
├─────────────────────────────────────────────────────────────────┤
│ UI: /training-plans/{id}/publish                                │
│                                                                 │
│ Usuario decide compartir su plan:                              │
│ 1. Click "Publicar en Marketplace"                             │
│ 2. Form:                                                        │
│    - Description para marketplace                              │
│    - Confirmar que objectives/exercises son compartibles       │
│ 3. Sistema crea MarketplaceItem:                               │
│    - Type = TrainingPlan                                       │
│    - Sport = Basketball                                        │
│    - SourceEntityId = plan.Id                                  │
│    - IsSystemOfficial = false                                  │
│                                                                 │
│ Resultado:                                                      │
│ → Plan visible en /marketplace para otros usuarios Basketball  │
│ → Puede recibir ratings/downloads                              │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 Flujo de Descarga desde Marketplace

```
┌─────────────────────────────────────────────────────────────────┐
│ PASO 1: Búsqueda en Marketplace                                │
├─────────────────────────────────────────────────────────────────┤
│ UI: /marketplace/browse                                         │
│                                                                 │
│ Filtros aplicados automáticamente:                             │
│ - Sport = Basketball (según subscription del usuario)          │
│                                                                 │
│ Filtros adicionales:                                            │
│ - Type: [TrainingPlan]                                         │
│ - Filter: TopRated                                             │
│ - SearchTerm: "pretemporada"                                   │
│                                                                 │
│ Resultados (paginados):                                         │
│ 1. "Plan Pretemporada Profesional" ⭐4.8 (120 downloads)       │
│    By: Club Barcelona (Oficial ✓)                              │
│                                                                 │
│ 2. "Preparación Base U16" ⭐4.5 (85 downloads)                 │
│    By: Coach John Smith                                        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 2: Ver Detalles                                           │
├─────────────────────────────────────────────────────────────────┤
│ UI: /marketplace/items/{id}                                     │
│                                                                 │
│ Información mostrada:                                           │
│ - Descripción completa                                         │
│ - Duración: 12 semanas, 36 sesiones                           │
│ - Objetivos incluidos: 8 objectives (lista expandible)        │
│ - Ratings: 4.8⭐ (24 valoraciones)                             │
│ - Reviews destacados                                           │
│ - Downloads: 120                                               │
│                                                                 │
│ Botón: [Descargar Plan]                                        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 3: Descarga e Importación                                 │
├─────────────────────────────────────────────────────────────────┤
│ API: POST /api/marketplace/download/{itemId}                   │
│                                                                 │
│ Domain Service: MarketplaceImportService.ImportPlan()          │
│                                                                 │
│ Proceso de clonación:                                           │
│ 1. Crear copia de TrainingPlan:                                │
│    - SubscriptionId = current user                             │
│    - Ownership = MarketplaceUser                               │
│    - SourceMarketplaceItemId = itemId                          │
│    - Name = "[Importado] " + original name                     │
│                                                                 │
│ 2. Para cada PlanObjective del plan original:                  │
│    a. Verificar si el Objective ya existe en user subscription │
│    b. Si NO existe:                                            │
│       - Clonar Objective:                                      │
│         * SubscriptionId = current user                        │
│         * Ownership = MarketplaceUser                          │
│         * SourceMarketplaceItemId = original objective         │
│       - Clonar ObjectiveTechniques asociadas                   │
│    c. Crear PlanObjective apuntando al objective (existente/nuevo)│
│                                                                 │
│ 3. Incrementar MarketplaceItem.TotalDownloads++                │
│ 4. Crear registro en marketplace_downloads (analytics)         │
│                                                                 │
│ Resultado:                                                      │
│ → Plan completo importado en la subscription del usuario       │
│ → Todos los objectives dependientes también importados         │
│ → Usuario puede ver el plan en /training-plans                 │
│ → Contenido es READ-ONLY (Ownership=MarketplaceUser)           │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ PASO 4: (Opcional) Clonar para Editar                          │
├─────────────────────────────────────────────────────────────────┤
│ UI: /training-plans/{importedPlanId}                            │
│                                                                 │
│ Usuario ve badge: "Contenido Importado (Solo Lectura)"         │
│                                                                 │
│ Botón: [Clonar y Editar]                                       │
│                                                                 │
│ API: POST /api/training-plans/{id}/clone                       │
│                                                                 │
│ Proceso:                                                        │
│ 1. Crear nueva copia del plan:                                 │
│    - Ownership = User (ahora es editable)                      │
│    - Name = "[Mi versión] " + original name                    │
│    - Copia todos los PlanObjectives                            │
│                                                                 │
│ 2. Usuario ahora puede:                                         │
│    - Modificar fechas/schedule                                 │
│    - Añadir/quitar objectives                                  │
│    - Generar workouts custom                                   │
└─────────────────────────────────────────────────────────────────┘
```

### 5.3 Flujo de Valoración en Marketplace

```
Usuario usa plan descargado → Entrenó con él 4 semanas → Decide valorar

UI: /marketplace/items/{id}
Botón: [Valorar este Plan]

Modal:
- Rating: ⭐⭐⭐⭐⭐ (5 estrellas - seleccionable)
- Comment: "Excelente progresión para U16. Los objetivos están muy bien estructurados."

API: POST /api/marketplace/rate
Request:
{
  "marketplaceItemId": "uuid",
  "stars": 5,
  "comment": "..."
}

Handler: RateMarketplaceItemCommand
1. Validar que user haya descargado el item (check marketplace_downloads)
2. Verificar que no haya valorado antes (UNIQUE constraint)
3. Crear MarketplaceRating
4. Re-calcular MarketplaceItem.AverageRating:
   - SQL: UPDATE marketplace_items SET
           average_rating = (SELECT AVG(stars) FROM marketplace_ratings WHERE ...)
           total_ratings = total_ratings + 1
5. Notificación al autor del contenido (futuro)

Resultado:
→ Rating visible en detalles del item
→ Average rating actualizado (4.8 → 4.82)
```

---

## 6. Especificación de API

### 6.1 Objectives API

**Base Path**: `/api/objectives`

#### `GET /api/objectives`
Obtener objetivos de la suscripción actual (User + MarketplaceUser content)

**Query Params**:
- `categoryId` (UUID, optional): Filtrar por categoría
- `subcategoryId` (UUID, optional): Filtrar por subcategoría
- `searchTerm` (string, optional): Búsqueda full-text
- `page` (int, default: 1)
- `pageSize` (int, default: 20, max: 100)

**Response 200**:
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "Cambio de mano rápido",
      "description": "...",
      "ownership": "User",
      "category": {
        "id": "uuid",
        "name": "Técnica Individual"
      },
      "subcategory": {
        "id": "uuid",
        "name": "Ataque"
      },
      "techniques": [
        {
          "id": "uuid",
          "description": "Mantener vista alta",
          "order": 1
        }
      ],
      "isEditable": true,
      "sourceMarketplaceItemId": null
    }
  ],
  "totalCount": 250,
  "page": 1,
  "pageSize": 20
}
```

---

#### `GET /api/objectives/system`
Obtener objetivos oficiales del sistema (filtrados por deporte de la suscripción)

**Query Params**:
- `categoryId` (UUID, optional)
- `subcategoryId` (UUID, optional)
- `searchTerm` (string, optional)
- `page` (int)
- `pageSize` (int)

**Response 200**: Same structure as above, pero `ownership = "System"`

**Cache**: Response cacheada 1 hora (Redis)

---

#### `POST /api/objectives`
Crear nuevo objetivo (User content)

**Request Body**:
```json
{
  "name": "Mi objetivo custom",
  "description": "Descripción detallada",
  "categoryId": "uuid",
  "subcategoryId": "uuid", // optional
  "techniques": [
    {
      "description": "Técnica 1",
      "order": 1
    },
    {
      "description": "Técnica 2",
      "order": 2
    }
  ]
}
```

**Response 201**:
```json
{
  "id": "uuid",
  "name": "Mi objetivo custom",
  // ... full objective
}
```

**Validations**:
- Name: Required, max 200 chars, unique per subscription
- CategoryId: Required, must exist
- SubcategoryId: Optional, must belong to specified category
- Techniques: Max 10 per objective

---

#### `PUT /api/objectives/{id}`
Actualizar objetivo existente (solo si Ownership=User)

**Request Body**: Same as POST

**Response 200**: Updated objective

**Response 403**: Si Ownership != User (no editable)

---

#### `POST /api/objectives/{id}/clone`
Clonar objetivo (System o MarketplaceUser → User editable)

**Response 201**: New objective con Ownership=User

---

#### `DELETE /api/objectives/{id}`
Soft delete de objetivo (solo User content)

**Response 204**: No content

**Response 403**: Si Ownership != User

**Response 409**: Si objetivo está en uso en algún TrainingPlan activo

---

### 6.2 Training Plans API

**Base Path**: `/api/training-plans`

#### `POST /api/training-plans`
Crear nueva planificación

**Request Body**:
```json
{
  "name": "Preparación Pretemporada 2025",
  "description": "Plan de 8 semanas para U16",
  "startDate": "2025-08-01",
  "endDate": "2025-09-30",
  "schedule": {
    "trainingDays": [1, 3, 5], // DayOfWeek enum (Monday=1, etc.)
    "hoursPerDay": {
      "1": 2,
      "3": 1.5,
      "5": 2
    }
  }
}
```

**Response 201**:
```json
{
  "id": "uuid",
  "name": "Preparación Pretemporada 2025",
  "startDate": "2025-08-01",
  "endDate": "2025-09-30",
  "schedule": {
    "trainingDays": [1, 3, 5],
    "hoursPerDay": { "1": 2, "3": 1.5, "5": 2 },
    "totalWeeks": 8,
    "totalSessions": 24,
    "totalHours": 132
  },
  "objectives": [],
  "createdAt": "2025-09-30T10:00:00Z"
}
```

---

#### `POST /api/training-plans/{id}/objectives`
Añadir objetivo al plan

**Request Body**:
```json
{
  "objectiveId": "uuid",
  "priority": 5, // 1-5
  "targetSessions": 7 // Opcional, si no se provee el sistema calcula basado en priority
}
```

**Response 200**: Plan actualizado con nuevo objetivo en lista

---

#### `DELETE /api/training-plans/{id}/objectives/{objectiveId}`
Quitar objetivo del plan

**Response 204**: No content

**Response 409**: Si ya hay workouts generados que usan este objetivo

---

#### `POST /api/training-plans/{id}/generate-workouts`
Generar todos los workouts del plan automáticamente

**Request Body** (opcional):
```json
{
  "regenerate": false // Si true, borra workouts existentes y regenera
}
```

**Response 202**: Accepted (proceso puede ser async si plan es muy grande)
```json
{
  "jobId": "uuid",
  "status": "Processing",
  "estimatedCompletion": "2025-09-30T10:05:00Z"
}
```

**Response 200** (si sync):
```json
{
  "workoutsCreated": 24,
  "workouts": [
    {
      "id": "uuid",
      "scheduledDate": "2025-08-05",
      "name": "Sesión 1: Cambio de mano + Defensa",
      "durationMinutes": 120,
      "objectivesCount": 2,
      "exercisesCount": 6
    }
    // ... 23 more
  ]
}
```

---

### 6.3 Workouts API

**Base Path**: `/api/workouts`

#### `GET /api/workouts`
Obtener workouts de la suscripción (con filtros)

**Query Params**:
- `trainingPlanId` (UUID, optional): Filtrar por plan
- `fromDate` (date, optional): Desde fecha
- `toDate` (date, optional): Hasta fecha
- `status` (WorkoutStatus, optional): Planned/InProgress/Completed/Cancelled
- `page`, `pageSize`

**Response 200**:
```json
{
  "items": [
    {
      "id": "uuid",
      "trainingPlanId": "uuid",
      "scheduledDate": "2025-08-05",
      "name": "Sesión 1: Cambio de mano + Defensa",
      "durationMinutes": 120,
      "status": "Planned",
      "objectives": [
        {
          "objectiveId": "uuid",
          "objectiveName": "Cambio de mano rápido",
          "timeAllocatedMinutes": 60
        }
      ],
      "exercises": [
        {
          "exerciseId": "uuid",
          "exerciseName": "1v1 con cambio",
          "order": 1,
          "durationMinutes": 15
        }
      ]
    }
  ]
}
```

---

#### `POST /api/workouts/{id}/start`
Iniciar ejecución de workout

**Response 200**:
```json
{
  "id": "uuid",
  "status": "InProgress",
  "actualStartTime": "2025-08-05T18:00:00Z"
}
```

**Response 409**: Si workout ya está InProgress o Completed

---

#### `POST /api/workouts/{id}/complete`
Finalizar workout

**Request Body**:
```json
{
  "notes": "Excelente sesión. Jugador X mostró gran mejora en cambios de mano."
}
```

**Response 200**:
```json
{
  "id": "uuid",
  "status": "Completed",
  "actualStartTime": "2025-08-05T18:00:00Z",
  "actualEndTime": "2025-08-05T20:05:00Z",
  "notes": "..."
}
```

---

### 6.4 Marketplace API

**Base Path**: `/api/marketplace`

#### `GET /api/marketplace/search`
Buscar items en marketplace

**Query Params**:
- `type` (MarketplaceItemType, optional): Objective/Exercise/Workout/TrainingPlan
- `filter` (MarketplaceFilter): All/OfficialOnly/CommunityOnly/Popular/TopRated
- `searchTerm` (string, optional)
- `page`, `pageSize`

**Response 200**:
```json
{
  "items": [
    {
      "id": "uuid",
      "type": "TrainingPlan",
      "sport": "Basketball",
      "name": "Plan Pretemporada Profesional",
      "description": "...",
      "isSystemOfficial": true,
      "averageRating": 4.8,
      "totalRatings": 24,
      "totalDownloads": 120,
      "publishedBy": "Club Barcelona",
      "publishedAt": "2025-01-15T00:00:00Z"
    }
  ],
  "totalCount": 156
}
```

---

#### `POST /api/marketplace/download/{itemId}`
Descargar e importar item

**Response 201**:
```json
{
  "importedEntityId": "uuid", // ID del entity clonado (Plan/Objective/etc.)
  "entityType": "TrainingPlan",
  "message": "Plan importado correctamente con 8 objetivos"
}
```

**Response 409**: Si el item ya fue descargado previamente (opcional: permitir re-download)

---

#### `POST /api/marketplace/publish`
Publicar contenido en marketplace

**Request Body**:
```json
{
  "entityType": "TrainingPlan",
  "entityId": "uuid",
  "description": "Descripción para marketplace (diferente a la interna)"
}
```

**Response 201**:
```json
{
  "marketplaceItemId": "uuid",
  "publishedAt": "2025-09-30T10:00:00Z",
  "url": "/marketplace/items/{uuid}"
}
```

**Validations**:
- Solo User content puede publicarse (Ownership=User)
- Entity debe pertenecer a la subscription del usuario
- Si es Plan, debe tener al menos 1 objetivo asignado

---

#### `POST /api/marketplace/rate`
Valorar item

**Request Body**:
```json
{
  "marketplaceItemId": "uuid",
  "stars": 5,
  "comment": "Excelente plan, muy bien estructurado"
}
```

**Response 201**: Rating creado

**Response 403**: Si usuario no ha descargado el item

**Response 409**: Si usuario ya valoró este item

---

## 7. Diseño de Frontend

### 7.1 Estructura de Features

```
src/app/features/
├── objectives/
│   ├── pages/
│   │   ├── objectives-list/
│   │   │   ├── objectives-list.component.ts
│   │   │   ├── objectives-list.component.html
│   │   │   └── objectives-list.component.scss
│   │   ├── objective-form/
│   │   │   └── ... (create/edit)
│   │   ├── system-objectives-browser/
│   │   │   └── ... (explore official library)
│   │   └── objective-details/
│   │       └── ...
│   ├── components/
│   │   ├── objective-card/
│   │   │   └── ... (reusable card)
│   │   ├── objective-technique-list/
│   │   │   └── ... (display techniques)
│   │   ├── objective-category-select/
│   │   │   └── ... (category/subcategory selector)
│   │   └── objective-clone-dialog/
│   │       └── ... (modal para clonar)
│   └── services/
│       └── objectives.service.ts
│
├── training-plans/
│   ├── pages/
│   │   ├── plans-list/
│   │   ├── plan-form/
│   │   ├── plan-details/
│   │   └── plan-schedule-builder/
│   ├── components/
│   │   ├── plan-card/
│   │   ├── plan-objectives-selector/ (modal con search)
│   │   ├── training-schedule-editor/ (visual drag&drop)
│   │   └── workout-generator-preview/
│   └── services/
│       └── training-plans.service.ts
│
├── workouts/
│   ├── pages/
│   │   ├── workouts-calendar/ (vista calendario con sesiones)
│   │   ├── workout-details/
│   │   └── workout-execution/ (cronómetro, checklist)
│   ├── components/
│   │   ├── workout-card/
│   │   ├── exercise-list/
│   │   ├── workout-timer/ (countdown timer)
│   │   └── workout-status-badge/
│   └── services/
│       └── workouts.service.ts
│
├── exercises/
│   ├── pages/
│   │   ├── exercises-library/ (grid con filtros)
│   │   ├── exercise-form/
│   │   └── exercise-details/
│   ├── components/
│   │   ├── exercise-card/
│   │   ├── exercise-video-player/
│   │   ├── exercise-difficulty-badge/
│   │   └── exercise-objectives-list/
│   └── services/
│       └── exercises.service.ts
│
└── marketplace/
    ├── pages/
    │   ├── marketplace-browse/ (main discovery)
    │   ├── marketplace-item-details/
    │   └── my-published-items/
    ├── components/
    │   ├── marketplace-search/ (filtros avanzados)
    │   ├── marketplace-item-card/
    │   ├── rating-stars/ (reusable star rating)
    │   ├── download-button/ (con feedback visual)
    │   └── rating-dialog/ (modal para valorar)
    └── services/
        └── marketplace.service.ts
```

### 7.2 Componentes Clave

#### ObjectiveCategorySelect Component

```typescript
@Component({
  selector: 'app-objective-category-select',
  standalone: true,
  imports: [MatFormFieldModule, MatSelectModule, ReactiveFormsModule],
  template: `
    <mat-form-field>
      <mat-label>Categoría</mat-label>
      <mat-select [formControl]="categoryControl" (selectionChange)="onCategoryChange()">
        <mat-option *ngFor="let cat of categories()" [value]="cat.id">
          {{ cat.name }}
        </mat-option>
      </mat-select>
    </mat-form-field>

    <mat-form-field *ngIf="subcategories().length > 0">
      <mat-label>Subcategoría</mat-label>
      <mat-select [formControl]="subcategoryControl">
        <mat-option *ngFor="let sub of subcategories()" [value]="sub.id">
          {{ sub.name }}
        </mat-option>
      </mat-select>
    </mat-form-field>
  `
})
export class ObjectiveCategorySelectComponent {
  categoryControl = input<FormControl>(new FormControl());
  subcategoryControl = input<FormControl>(new FormControl());

  categories = signal<Category[]>([]);
  subcategories = signal<Subcategory[]>([]);

  private objectivesService = inject(ObjectivesService);

  ngOnInit() {
    this.objectivesService.getCategories().subscribe(cats => this.categories.set(cats));
  }

  onCategoryChange() {
    const categoryId = this.categoryControl().value;
    this.objectivesService.getSubcategories(categoryId).subscribe(subs => {
      this.subcategories.set(subs);
      this.subcategoryControl().setValue(null);
    });
  }
}
```

#### TrainingScheduleEditor Component

```typescript
@Component({
  selector: 'app-training-schedule-editor',
  standalone: true,
  template: `
    <div class="schedule-editor">
      <h3>Selecciona días de entrenamiento</h3>
      <div class="days-selector">
        <mat-checkbox *ngFor="let day of daysOfWeek"
                      [checked]="isDaySelected(day)"
                      (change)="toggleDay(day)">
          {{ day.name }}
        </mat-checkbox>
      </div>

      <h3>Horas por día</h3>
      <div class="hours-inputs">
        <mat-form-field *ngFor="let day of selectedDays()">
          <mat-label>{{ day.name }}</mat-label>
          <input matInput type="number" min="0.5" max="8" step="0.5"
                 [value]="hoursPerDay()[day.value]"
                 (input)="updateHours(day.value, $event)" />
          <span matSuffix>horas</span>
        </mat-form-field>
      </div>

      <div class="summary" *ngIf="schedule()">
        <mat-card>
          <mat-card-header>
            <mat-card-title>Resumen</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p><strong>Total sesiones:</strong> {{ schedule().totalSessions }}</p>
            <p><strong>Total horas:</strong> {{ schedule().totalHours }}h</p>
            <p><strong>Duración:</strong> {{ schedule().totalWeeks }} semanas</p>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `
})
export class TrainingScheduleEditorComponent {
  startDate = input.required<Date>();
  endDate = input.required<Date>();
  scheduleChange = output<TrainingSchedule>();

  selectedDays = signal<Day[]>([]);
  hoursPerDay = signal<Record<number, number>>({});
  schedule = signal<TrainingSchedule | null>(null);

  daysOfWeek = [
    { value: 1, name: 'Lunes' },
    { value: 2, name: 'Martes' },
    { value: 3, name: 'Miércoles' },
    { value: 4, name: 'Jueves' },
    { value: 5, name: 'Viernes' },
    { value: 6, name: 'Sábado' },
    { value: 0, name: 'Domingo' }
  ];

  toggleDay(day: Day) {
    const current = this.selectedDays();
    if (this.isDaySelected(day)) {
      this.selectedDays.set(current.filter(d => d.value !== day.value));
    } else {
      this.selectedDays.set([...current, day]);
      // Default 2 hours per day
      this.hoursPerDay.update(hours => ({ ...hours, [day.value]: 2 }));
    }
    this.recalculateSchedule();
  }

  updateHours(dayValue: number, event: any) {
    const hours = parseFloat(event.target.value);
    this.hoursPerDay.update(current => ({ ...current, [dayValue]: hours }));
    this.recalculateSchedule();
  }

  private recalculateSchedule() {
    const days = this.selectedDays().map(d => d.value);
    const hours = this.hoursPerDay();

    // Calculate totals
    const totalDays = (this.endDate().getTime() - this.startDate().getTime()) / (1000 * 60 * 60 * 24);
    const totalWeeks = Math.ceil(totalDays / 7);

    // Count training sessions
    let totalSessions = 0;
    let date = new Date(this.startDate());
    while (date <= this.endDate()) {
      if (days.includes(date.getDay())) {
        totalSessions++;
      }
      date.setDate(date.getDate() + 1);
    }

    const totalHours = Object.values(hours).reduce((sum, h) => sum + h, 0) * (totalSessions / days.length);

    const schedule: TrainingSchedule = {
      trainingDays: days,
      hoursPerDay: hours,
      totalWeeks,
      totalSessions,
      totalHours
    };

    this.schedule.set(schedule);
    this.scheduleChange.emit(schedule);
  }
}
```

#### WorkoutCalendarComponent

```typescript
@Component({
  selector: 'app-workouts-calendar',
  standalone: true,
  imports: [FullCalendarModule, MatButtonModule],
  template: `
    <div class="calendar-container">
      <full-calendar [options]="calendarOptions()"></full-calendar>
    </div>

    <app-workout-details-dialog
      [workout]="selectedWorkout()"
      (close)="selectedWorkout.set(null)"
      (start)="startWorkout($event)"
      (edit)="editWorkout($event)" />
  `
})
export class WorkoutsCalendarComponent {
  private workoutsService = inject(WorkoutsService);
  private router = inject(Router);

  workouts = signal<Workout[]>([]);
  selectedWorkout = signal<Workout | null>(null);

  calendarOptions = computed(() => ({
    initialView: 'dayGridMonth',
    events: this.workouts().map(w => ({
      id: w.id,
      title: w.name,
      start: w.scheduledDate,
      backgroundColor: this.getColorByStatus(w.status),
      extendedProps: { workout: w }
    })),
    eventClick: (info: any) => {
      this.selectedWorkout.set(info.event.extendedProps.workout);
    }
  }));

  ngOnInit() {
    this.workoutsService.getUpcoming().subscribe(workouts => {
      this.workouts.set(workouts);
    });
  }

  startWorkout(workout: Workout) {
    this.router.navigate(['/workouts', workout.id, 'execute']);
  }

  getColorByStatus(status: WorkoutStatus): string {
    switch (status) {
      case WorkoutStatus.Planned: return '#4CAF50';
      case WorkoutStatus.InProgress: return '#FF9800';
      case WorkoutStatus.Completed: return '#2196F3';
      case WorkoutStatus.Cancelled: return '#9E9E9E';
    }
  }
}
```

---

## 8. Estrategia de Testing

### 8.1 Unit Tests (Domain Layer)

**Objetivo**: >80% coverage en Domain y Application layers

**Ejemplo: Objective.cs**

```csharp
public class ObjectiveTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesObjective()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Test Objective";

        // Act
        var objective = new Objective(
            subscriptionId,
            ContentOwnership.User,
            Sport.Basketball,
            name,
            "Description",
            categoryId,
            null
        );

        // Assert
        objective.SubscriptionId.Should().Be(subscriptionId);
        objective.Name.Should().Be(name);
        objective.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddTechnique_WhenCalled_AddsToCollection()
    {
        // Arrange
        var objective = CreateValidObjective();
        var technique = new ObjectiveTechnique(objective.Id, "Technique 1", 1);

        // Act
        objective.AddTechnique(technique);

        // Assert
        objective.Techniques.Should().Contain(technique);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Arrange & Act
        var act = () => new Objective(
            Guid.NewGuid(),
            ContentOwnership.User,
            Sport.Basketball,
            invalidName,
            "Description",
            Guid.NewGuid(),
            null
        );

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
```

**Ejemplo: WorkoutAutoGeneratorService**

```csharp
public class WorkoutAutoGeneratorServiceTests
{
    private readonly Mock<IObjectiveRepository> _objectiveRepoMock;
    private readonly Mock<IExerciseRepository> _exerciseRepoMock;
    private readonly WorkoutAutoGeneratorService _sut;

    public WorkoutAutoGeneratorServiceTests()
    {
        _objectiveRepoMock = new Mock<IObjectiveRepository>();
        _exerciseRepoMock = new Mock<IExerciseRepository>();
        _sut = new WorkoutAutoGeneratorService(_objectiveRepoMock.Object, _exerciseRepoMock.Object);
    }

    [Fact]
    public async Task GenerateWorkouts_WithValidPlan_CreatesCorrectNumberOfWorkouts()
    {
        // Arrange
        var plan = CreatePlanWith3DaysPerWeek(startDate: new DateTime(2025, 8, 1), endDate: new DateTime(2025, 8, 31));
        var objectives = CreateObjectivesWithPriorities(plan.Id);
        var exercises = CreateExercisesForObjectives(objectives);

        _objectiveRepoMock.Setup(r => r.GetByPlanIdAsync(plan.Id)).ReturnsAsync(objectives);
        _exerciseRepoMock.Setup(r => r.GetByObjectivesAsync(It.IsAny<List<Guid>>())).ReturnsAsync(exercises);

        // Act
        var workouts = await _sut.GenerateAllWorkoutsAsync(plan);

        // Assert
        workouts.Should().HaveCount(12); // 3 días/semana × 4 semanas
        workouts.All(w => w.DurationMinutes > 0).Should().BeTrue();
        workouts.All(w => w.Exercises.Any()).Should().BeTrue();
    }

    [Fact]
    public async Task GenerateWorkouts_DistributesObjectivesByPriority()
    {
        // Arrange
        var plan = CreatePlanWithObjectives(
            new PlanObjective { Priority = 5, TargetSessions = 10 },
            new PlanObjective { Priority = 3, TargetSessions = 5 }
        );

        // Act
        var workouts = await _sut.GenerateAllWorkoutsAsync(plan);

        // Assert
        var highPriorityCount = workouts.Count(w => w.Objectives.Any(o => o.Priority == 5));
        var lowPriorityCount = workouts.Count(w => w.Objectives.Any(o => o.Priority == 3));

        highPriorityCount.Should().BeGreaterThan(lowPriorityCount);
    }
}
```

### 8.2 Integration Tests (API Layer)

**Ejemplo: ObjectivesController Integration Test**

```csharp
public class ObjectivesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly SportPlannerDbContext _dbContext;

    public ObjectivesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _dbContext = factory.Services.GetRequiredService<SportPlannerDbContext>();
    }

    [Fact]
    public async Task CreateObjective_WithValidData_ReturnsCreatedObjective()
    {
        // Arrange
        var subscriptionId = await SeedTestSubscription();
        AuthenticateClient(subscriptionId);

        var request = new CreateObjectiveRequest
        {
            Name = "Integration Test Objective",
            Description = "Test",
            CategoryId = await GetBasketballCategoryId(),
            Techniques = new List<TechniqueDto>
            {
                new TechniqueDto { Description = "Technique 1", Order = 1 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/objectives", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var objective = await response.Content.ReadFromJsonAsync<ObjectiveResponse>();
        objective.Name.Should().Be(request.Name);
        objective.Techniques.Should().HaveCount(1);

        // Verify DB
        var dbObjective = await _dbContext.Objectives.FindAsync(objective.Id);
        dbObjective.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSystemObjectives_FiltersBySport()
    {
        // Arrange
        await SeedSystemObjectives(Sport.Basketball, count: 10);
        await SeedSystemObjectives(Sport.Football, count: 5);

        var basketballSubscriptionId = await SeedTestSubscription(Sport.Basketball);
        AuthenticateClient(basketballSubscriptionId);

        // Act
        var response = await _client.GetAsync("/api/objectives/system");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ObjectiveResponse>>();
        result.Items.Should().HaveCount(10); // Solo Basketball
        result.Items.All(o => o.Sport == Sport.Basketball).Should().BeTrue();
    }
}
```

### 8.3 E2E Tests (Playwright)

**Ejemplo: Full flow de planificación**

```typescript
import { test, expect } from '@playwright/test';

test.describe('Training Plan Creation Flow', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.fill('[name="email"]', 'test@example.com');
    await page.fill('[name="password"]', 'password123');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL('/dashboard');
  });

  test('Create plan, add objectives, generate workouts', async ({ page }) => {
    // Step 1: Navigate to plans
    await page.click('text=Planificaciones');
    await expect(page).toHaveURL('/training-plans');

    // Step 2: Create new plan
    await page.click('button:has-text("Nueva Planificación")');
    await page.fill('[name="name"]', 'E2E Test Plan');
    await page.fill('[name="startDate"]', '2025-08-01');
    await page.fill('[name="endDate"]', '2025-09-30');

    // Select training days
    await page.check('text=Lunes');
    await page.check('text=Miércoles');
    await page.check('text=Viernes');

    // Set hours
    await page.fill('[name="hoursMonday"]', '2');
    await page.fill('[name="hoursWednesday"]', '1.5');
    await page.fill('[name="hoursFriday"]', '2');

    await page.click('button:has-text("Crear Plan")');
    await expect(page).toHaveURL(/\/training-plans\/[a-f0-9-]+/);

    // Step 3: Add objectives
    await page.click('button:has-text("Añadir Objetivos")');

    // Search and select from system objectives
    await page.fill('[placeholder="Buscar objetivos"]', 'cambio de mano');
    await page.click('mat-option:has-text("Cambio de mano rápido")');
    await page.selectOption('[name="priority"]', '5');
    await page.click('button:has-text("Añadir")');

    // Verify objective added
    await expect(page.locator('text=Cambio de mano rápido')).toBeVisible();

    // Step 4: Generate workouts
    await page.click('button:has-text("Generar Entrenamientos")');
    await expect(page.locator('text=Generando entrenamientos')).toBeVisible();

    // Wait for generation
    await page.waitForSelector('text=24 entrenamientos generados', { timeout: 10000 });

    // Step 5: View calendar
    await page.click('text=Ver Calendario');
    await expect(page).toHaveURL('/workouts/calendar');

    // Verify workouts in calendar
    const workoutEvents = page.locator('.fc-event');
    await expect(workoutEvents).toHaveCount(24);

    // Step 6: Open a workout
    await workoutEvents.first().click();
    await expect(page.locator('[role="dialog"]')).toBeVisible();
    await expect(page.locator('text=Sesión 1')).toBeVisible();
  });

  test('Download plan from marketplace and use it', async ({ page }) => {
    // Navigate to marketplace
    await page.click('text=Marketplace');
    await expect(page).toHaveURL('/marketplace');

    // Filter by Training Plans
    await page.selectOption('[name="type"]', 'TrainingPlan');
    await page.selectOption('[name="filter"]', 'TopRated');

    // Click on first plan
    await page.click('.marketplace-item-card').first();

    // Download
    await page.click('button:has-text("Descargar Plan")');
    await expect(page.locator('text=Plan importado correctamente')).toBeVisible();

    // Navigate to imported plan
    await page.click('text=Ver Plan Importado');
    await expect(page).toHaveURL(/\/training-plans\/[a-f0-9-]+/);

    // Verify it's read-only
    await expect(page.locator('text=Contenido Importado (Solo Lectura)')).toBeVisible();

    // Clone to edit
    await page.click('button:has-text("Clonar y Editar")');
    await expect(page.locator('text=Plan clonado correctamente')).toBeVisible();

    // Now editable
    await expect(page.locator('button:has-text("Editar Plan")')).toBeEnabled();
  });
});
```

---

## 9. Plan de Implementación por Fases

### FASE 0: System Content Seed (3-5 días)

**Objetivo**: Preparar contenido oficial por deporte

**Tareas**:
1. Definir estructura de contenido oficial:
   - Basketball: 250 objectives + 150 exercises
   - Football: 250 objectives + 150 exercises
   - Handball: 250 objectives + 150 exercises

2. Crear CSVs/JSON con contenido:
   ```csv
   sport,category,subcategory,name,description,techniques
   Basketball,Técnica Individual,Ataque,"Cambio de mano rápido","...","Mantener vista alta|Cambio explosivo|..."
   ```

3. Crear script de importación:
   ```csharp
   public class SystemContentImporter
   {
       public async Task ImportFromCsv(string filePath, Sport sport)
       {
           // Parse CSV
           // Create Objective entities con Ownership=System, SubscriptionId=null
           // Bulk insert
       }
   }
   ```

4. Crear migrations con seed data:
   ```csharp
   public class SeedBasketballContent : Migration
   {
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           // INSERT masivo de objectives
           // INSERT masivo de objective_techniques
           // INSERT masivo de exercises
           // INSERT masivo de exercise_objectives
       }
   }
   ```

**Entregable**: Migrations ejecutables con 750+ objectives y 450+ exercises

---

### FASE 1: Domain + Application - Objectives (2-3 días)

**Tareas**:

1. **Domain Entities** (1 día):
   - `Objective.cs` con validaciones
   - `ObjectiveCategory.cs`, `ObjectiveSubcategory.cs`
   - `ObjectiveTechnique.cs`
   - `ContentOwnership` enum

2. **Application Layer** (1-2 días):
   - `CreateObjectiveCommand` + Handler + Validator
   - `UpdateObjectiveCommand` + Handler
   - `CloneObjectiveCommand` + Handler (System/Marketplace → User)
   - `GetObjectivesBySubscriptionQuery` + Handler
   - `GetSystemObjectivesBySportQuery` + Handler (con cache)

3. **Tests** (integrado):
   - Unit tests para Objective entity
   - Unit tests para command handlers (con mocks)

**Entregable**: Domain + Application completos para Objectives

---

### FASE 2: Infrastructure + API - Objectives (2 días)

**Tareas**:

1. **Infrastructure** (1 día):
   - Migration: `AddObjectiveEntities`
   - `ObjectiveRepository` (implementación)
   - `ObjectiveCategoryRepository`
   - EF Core configurations (`ObjectiveConfiguration.cs`)
   - Cache layer para system objectives (IDistributedCache)

2. **API Controller** (1 día):
   - `ObjectivesController` con todos los endpoints
   - Swagger documentation
   - Authorization (JWT + Subscription validation)

3. **Tests**:
   - Integration tests para controller
   - Test de cache hit/miss

**Entregable**: API funcional de Objectives con seed data cargado

---

### FASE 3: Domain + Application - Training Plans (3 días)

**Tareas**:

1. **Domain Entities** (1 día):
   - `TrainingPlan.cs`
   - `TrainingSchedule.cs` (Value Object con cálculos)
   - `PlanObjective.cs` (junction con metadata)

2. **Application Layer** (2 días):
   - `CreateTrainingPlanCommand` + Handler
   - `UpdateTrainingPlanCommand` + Handler
   - `AddObjectiveToPlanCommand` + Handler
   - `RemoveObjectiveFromPlanCommand` + Handler
   - `UpdateObjectivePriorityCommand` + Handler
   - `GetPlansBySubscriptionQuery` + Handler
   - `GetPlanDetailsQuery` + Handler

3. **Tests**:
   - TrainingSchedule calculations unit tests
   - Command handlers con mocks

**Entregable**: Domain + Application de Training Plans

---

### FASE 4: Infrastructure + API - Training Plans (2 días)

**Tareas**:

1. **Infrastructure**:
   - Migration: `AddTrainingPlanEntities`
   - `TrainingPlanRepository`
   - EF Core configurations

2. **API Controller**:
   - `TrainingPlansController` con endpoints CRUD
   - Endpoints de gestión de objectives

3. **Tests**:
   - Integration tests

**Entregable**: API funcional de Training Plans

---

### FASE 5: Domain - Workouts Generation Service (3 días)

**Tareas**:

1. **Entities** (1 día):
   - `Workout.cs`
   - `WorkoutObjective.cs`
   - `WorkoutExercise.cs`
   - `Exercise.cs`, `ExerciseObjective.cs`
   - `WorkoutStatus` enum

2. **Domain Service** (2 días):
   - `WorkoutAutoGeneratorService`:
     - `GenerateAllWorkoutsAsync(TrainingPlan plan)`
     - `GenerateNextWorkoutAsync(TrainingPlan plan)`
     - Lógica de distribución de objectives
     - Algoritmo de selección de exercises
     - Balanceo de dificultad

3. **Tests**:
   - Unit tests exhaustivos de generación
   - Casos edge: plan sin objectives, sin exercises disponibles, etc.

**Entregable**: Lógica de generación automática funcionando

---

### FASE 6: Application + Infrastructure + API - Workouts & Exercises (3 días)

**Tareas**:

1. **Application Layer** (1 día):
   - `GenerateWorkoutsFromPlanCommand` + Handler (usa service)
   - `CreateExerciseCommand` + Handler
   - `StartWorkoutCommand` + Handler
   - `CompleteWorkoutCommand` + Handler
   - `GetUpcomingWorkoutsQuery` + Handler
   - `GetWorkoutDetailsQuery` + Handler

2. **Infrastructure** (1 día):
   - Migrations: `AddWorkoutAndExerciseEntities`
   - Repositories: `WorkoutRepository`, `ExerciseRepository`
   - Configurations EF Core

3. **API Controllers** (1 día):
   - `WorkoutsController`
   - `ExercisesController`

4. **Tests**:
   - Integration tests end-to-end (create plan → generate workouts → start → complete)

**Entregable**: Sistema completo de generación y ejecución de workouts

---

### FASE 7: Marketplace Backend (3 días)

**Tareas**:

1. **Domain + Application** (1.5 días):
   - `MarketplaceItem.cs`, `MarketplaceRating.cs`, `MarketplaceDownload.cs`
   - `PublishToMarketplaceCommand<T>` + Handler (genérico)
   - `SearchMarketplaceQuery` + Handler (con filtros)
   - `DownloadFromMarketplaceCommand<T>` + Handler (lógica de clonación)
   - `RateMarketplaceItemCommand` + Handler

2. **Infrastructure** (0.5 día):
   - Migration: `AddMarketplaceEntities`
   - `MarketplaceRepository`
   - Índices de performance

3. **API Controller** (1 día):
   - `MarketplaceController` con todos los endpoints
   - Validaciones de ownership

4. **Tests**:
   - Integration test: publish → search → download → rate

**Entregable**: Marketplace completamente funcional

---

### FASE 8: Frontend - Objectives & Plans (4 días)

**Tareas**:

1. **Objectives Feature** (2 días):
   - `objectives-list.component` (con infinite scroll)
   - `objective-form.component` (create/edit)
   - `system-objectives-browser.component` (explorar biblioteca oficial)
   - `objective-clone-dialog.component`
   - `ObjectivesService` con cache local

2. **Training Plans Feature** (2 días):
   - `plans-list.component`
   - `plan-form.component`
   - `plan-schedule-editor.component` (visual builder)
   - `plan-objectives-selector.component` (modal con search)
   - `TrainingPlansService`

3. **UI/UX**:
   - Material Design components
   - Tailwind CSS styling
   - Responsive mobile-first
   - Loading states

**Entregable**: UI completa de Objectives y Plans

---

### FASE 9: Frontend - Workouts & Exercises (4 días)

**Tareas**:

1. **Workouts Feature** (2 días):
   - `workouts-calendar.component` (FullCalendar integration)
   - `workout-details.component`
   - `workout-execution.component` (con timer)
   - `WorkoutsService`

2. **Exercises Feature** (2 días):
   - `exercises-library.component` (grid con filtros)
   - `exercise-form.component`
   - `exercise-card.component`
   - `ExercisesService`

3. **Animations**:
   - Transiciones Material
   - Timer countdown animation

**Entregable**: UI completa de Workouts y Exercises

---

### FASE 10: Frontend - Marketplace (3 días)

**Tareas**:

1. **Marketplace Feature** (2 días):
   - `marketplace-browse.component` (infinite scroll)
   - `marketplace-item-details.component`
   - `marketplace-rating-dialog.component`
   - `my-published-items.component`
   - `MarketplaceService`

2. **Shared Components** (1 día):
   - `rating-stars.component` (reusable)
   - `download-button.component` (con feedback)

**Entregable**: UI completa de Marketplace

---

### FASE 11: Testing & Polish (3 días)

**Tareas**:

1. **E2E Tests** (2 días):
   - Playwright tests para flujos completos
   - Coverage de happy path + edge cases

2. **Performance Testing** (0.5 día):
   - Load testing de búsqueda marketplace (10K+ items)
   - Cache effectiveness validation

3. **UX Refinement** (0.5 día):
   - Error handling consistency
   - Loading states
   - Empty states
   - Success feedback

4. **Accessibility Audit** (0.5 día):
   - WCAG 2.1 AA compliance
   - Keyboard navigation
   - Screen reader testing

**Entregable**: Sistema production-ready

---

## 10. Consideraciones de Performance

### 10.1 Caching Strategy

**System Content** (Objectives/Exercises oficiales):

```csharp
public class CachedObjectiveRepository : IObjectiveRepository
{
    private readonly IObjectiveRepository _innerRepository;
    private readonly IDistributedCache _cache;
    private const string SYSTEM_OBJECTIVES_KEY = "objectives:system:{sport}";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<List<Objective>> GetSystemObjectivesBySportAsync(Sport sport)
    {
        var cacheKey = SYSTEM_OBJECTIVES_KEY.Replace("{sport}", sport.ToString());

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<Objective>>(cached);
        }

        var objectives = await _innerRepository.GetSystemObjectivesBySportAsync(sport);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(objectives),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration }
        );

        return objectives;
    }
}
```

**Marketplace Search Results**:
- Cache de resultados populares (5 min TTL)
- Invalidación al publicar nuevo item

### 10.2 Database Indexing

**Índices críticos ya definidos en Schema SQL** (sección 4.1).

**Query Optimization**:

```sql
-- Búsqueda de exercises por objective (M:M join optimizado)
SELECT e.*
FROM exercises e
INNER JOIN exercise_objectives eo ON e.id = eo.exercise_id
WHERE eo.objective_id = ANY($1) -- Array de objective IDs
  AND e.sport = $2
  AND e.is_active = true
ORDER BY e.difficulty, e.duration_minutes
LIMIT 20;
```

### 10.3 Paginación

**Server-Side Paging** para todas las listas grandes:

```csharp
public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
{
    var query = _dbContext.Set<T>().AsQueryable();

    var totalCount = await query.CountAsync(ct);

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    return new PagedResult<T>
    {
        Items = items,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    };
}
```

**Frontend Infinite Scroll** para mejor UX en mobile.

### 10.4 Async Generation

Para planes grandes (50+ workouts), generación asíncrona:

```csharp
public class GenerateWorkoutsCommandHandler : IRequestHandler<GenerateWorkoutsCommand, Guid>
{
    private readonly IBackgroundJobClient _backgroundJobs;

    public async Task<Guid> Handle(GenerateWorkoutsCommand request, CancellationToken ct)
    {
        // Si plan es grande, procesar en background
        if (EstimatedWorkoutsCount(request.PlanId) > 30)
        {
            var jobId = _backgroundJobs.Enqueue<WorkoutGenerationJob>(
                job => job.GenerateAsync(request.PlanId)
            );
            return jobId;
        }

        // Si es pequeño, procesar síncronamente
        await _generatorService.GenerateAllWorkoutsAsync(request.PlanId);
        return Guid.Empty; // Indica procesamiento sync
    }
}
```

---

## 11. Seguridad y Multi-tenancy

### 11.1 Isolation por Subscription

**Global Query Filter en EF Core**:

```csharp
public class SportPlannerDbContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global filter: solo ver entidades de tu subscription
        modelBuilder.Entity<Objective>().HasQueryFilter(o =>
            o.SubscriptionId == null || // System content
            o.SubscriptionId == _currentUser.SubscriptionId
        );

        modelBuilder.Entity<TrainingPlan>().HasQueryFilter(p =>
            p.SubscriptionId == _currentUser.SubscriptionId
        );

        // ... apply to all tenant-scoped entities
    }
}
```

### 11.2 Authorization Policies

```csharp
public class ObjectiveAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Objective>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Objective resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var subscriptionId = context.User.FindFirstValue("subscription_id");

        // System content: solo read
        if (resource.Ownership == ContentOwnership.System && requirement.Name == "Read")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // User content: owner puede edit/delete
        if (resource.Ownership == ContentOwnership.User &&
            resource.SubscriptionId.ToString() == subscriptionId)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // MarketplaceUser content: solo read
        if (resource.Ownership == ContentOwnership.MarketplaceUser &&
            resource.SubscriptionId.ToString() == subscriptionId &&
            requirement.Name == "Read")
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

### 11.3 Rate Limiting

```csharp
// Marketplace publishing rate limit
services.AddRateLimiter(options =>
{
    options.AddPolicy("marketplace-publish", context =>
    {
        var subscriptionId = context.User.FindFirstValue("subscription_id");

        return RateLimitPartition.GetSlidingWindowLimiter(subscriptionId, _ =>
            new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10, // 10 publicaciones
                Window = TimeSpan.FromDays(1), // por día
                SegmentsPerWindow = 4
            });
    });
});

// Apply to controller
[ApiController]
[Route("api/marketplace")]
public class MarketplaceController : ControllerBase
{
    [HttpPost("publish")]
    [EnableRateLimiting("marketplace-publish")]
    public async Task<IActionResult> Publish([FromBody] PublishRequest request) { ... }
}
```

---

## 12. Roadmap y Extensiones Futuras

### 12.1 MVP (Incluido en Plan Actual)

✅ Objectives con library oficial
✅ Training Plans con schedule visual
✅ Generación automática de workouts
✅ Exercises con link a objectives
✅ Marketplace básico con ratings
✅ Multi-tenancy completo

### 12.2 Fase 2 (Post-MVP, +3 meses)

**Analytics & Insights**:
- Dashboard de progreso: % completación de objectives
- Heatmap de entrenamientos completados
- Estadísticas de uso de exercises (más usados, mejor valorados)

**AI-Powered Generation**:
- GPT-4 integration para generar descriptions de exercises
- Sugerencias inteligentes de objectives basadas en historial
- Auto-ajuste de planificación según progreso real

**Social Features**:
- Comentarios en marketplace items
- Follow a otros entrenadores
- Share workouts via link público

**Mobile App**:
- Angular Capacitor o React Native
- Workout execution con push notifications
- Offline mode para entrenamientos

### 12.3 Fase 3 (Post-MVP, +6 meses)

**Player Management**:
- Entity `Player` vinculada a Teams
- Asignación de objectives a players individuales
- Tracking de progreso individual
- Injury management

**Video Integration**:
- Upload de videos de exercises (S3/Azure Blob)
- Video analysis con AI (pose detection)

**Advanced Search**:
- Migración a Elasticsearch para búsqueda avanzada
- Filtros por tags, difficulty, duración, equipment
- Recommendations: "Si te gustó este plan, prueba..."

**Gamification**:
- Badges por completar X workouts
- Leaderboards de entrenadores (más contenido publicado/valorado)
- Challenges mensuales

---

## 13. Conclusión

Este documento especifica completamente el sistema de **Objectives, Planning y Marketplace** para SportPlanner, con:

- ✅ **Arquitectura Domain-Driven** clara y escalable
- ✅ **Plan de implementación por fases** realista (7-8 semanas)
- ✅ **Modelo de datos completo** con 14 nuevas tablas
- ✅ **Separación System/User content** con marketplace
- ✅ **Generación automática inteligente** de workouts
- ✅ **Testing strategy** con >80% coverage
- ✅ **Performance optimization** (cache, índices, paginación)
- ✅ **Security multi-tenant** con authorization policies

**Próximos Pasos**:
1. Revisar y aprobar este diseño
2. Crear ADR oficial (ya creado en `docs/adr/`)
3. Iniciar FASE 0: Seed data preparation
4. Comenzar desarrollo iterativo por fases

---

**Documento Mantenido Por**: Development Team
**Última Actualización**: 2025-09-30
**Versión**: 1.0
**Estado**: ✅ Listo para Aprobación