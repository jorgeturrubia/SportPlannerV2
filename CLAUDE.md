# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Quality Gate

**CRITICAL: Before any code changes, you MUST read and comply with ALL rules in `.clinerules/` folder.**

Complete the mandatory checklist from `.clinerules/00-development-gate.md` before writing any code. This ensures compliance with architecture, security, testing, and code quality standards.

## Project Architecture

### Full-Stack Architecture

This is a full-stack SportPlanner application with:

- **Frontend**: Angular 20 with standalone components (no NgModules)
- **Backend**: .NET 8 with Clean Architecture
- **Authentication**: Supabase JWT with JWKS validation
- **Database**: PostgreSQL with Entity Framework Core
- **UI Framework**: Angular Material + Tailwind CSS v4

### Directory Structure

```
src/
├── front/SportPlanner/          # Angular 20 frontend
│   ├── src/app/features/        # Feature-based architecture
│   ├── src/custom-theme.scss    # Material Design theme
│   └── package.json
├── back/SportPlanner/           # .NET 8 backend
│   ├── src/SportPlanner.Domain/      # Domain entities
│   ├── src/SportPlanner.Application/ # Use cases/commands
│   ├── src/SportPlanner.Infrastructure/ # Data access
│   ├── src/SportPlanner.API/         # Controllers/endpoints
│   └── tests/                        # Unit & integration tests
├── docs/adr/                    # Architecture Decision Records
└── .clinerules/                 # Development standards
```

## Common Development Commands

**Note**: All paths are relative to `src/` directory. Commands assume Windows environment.

### Frontend (Angular)

```bash
# Navigate to frontend
cd front/SportPlanner

# Start development server (runs on http://localhost:4200)
npm start

# Build for production
npm run build

# Run all tests
npm test

# Run tests with coverage
npm test -- --no-watch --code-coverage

# Watch mode build
npm run watch

# Generate component (from front/SportPlanner directory)
ng generate component features/training/components/component-name
```

### Backend (.NET)

```bash
# Navigate to backend
cd back/SportPlanner

# Build solution
dotnet build

# Run API (starts on configured port, typically https://localhost:5001)
dotnet run --project src/SportPlanner.API

# Watch mode for API (auto-reload on file changes)
dotnet watch run --project src/SportPlanner.API

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/SportPlanner.Domain.UnitTests

# Run single test class
dotnet test --filter "FullyQualifiedName~ClassName"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Add migration
dotnet ef migrations add MigrationName --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API

# Update database
dotnet ef database update --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API

# Remove last migration
dotnet ef migrations remove --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
```

## Architecture Patterns

### Frontend Architecture

- **Standalone Components**: All components must be standalone (no NgModules)
- **Feature-Based Structure**: Organize by features (training, athlete, coach, planning)
- **Lazy Loading**: Use `loadComponent` for route-level code splitting
- **Signals**: Use Angular signals for reactive state management
- **Material + Tailwind**: Angular Material components with Tailwind utility classes

### Backend Architecture

- **Clean Architecture**: Domain → Application → Infrastructure → API layers
- **Vertical Slice**: Organize by features/use cases, not technical concerns
- **CQRS with MediatR**: Commands and queries separated using MediatR
- **Entity Framework**: Direct DbContext usage unless complex domain rules require repositories
- **JWT Authentication**: Supabase JWT validation with automatic JWKS fetching

### Key Architectural Rules

1. **No NgModules**: Use standalone components exclusively
2. **No Custom JWT**: Always use Supabase authentication with JWKS validation
3. **Feature-Based Organization**: Group by business features, not technical layers
4. **Dependency Injection**: Use Angular's `inject()` function and .NET DI container
5. **Clean Separation**: Never mix UI logic with business logic
6. **Testing Required**: Minimum 80% coverage for Application/Domain layers

## Authentication & Security

### Supabase JWT Integration

- **Frontend**: Use Supabase client SDK (`@supabase/supabase-js`) for authentication
- **Backend**: JWT Bearer authentication configured with Authority pointing to Supabase project URL
- **JWKS**: Automatic key validation against `https://<project-id>.supabase.co/.well-known/jwks.json`
- **Claims**: Role-based authorization using JWT claims (issuer, audience must match Supabase)
- **Configuration**: Supabase URL and anon key required in environment variables

### Security Standards

- Never store tokens in localStorage/sessionStorage
- Use HttpOnly cookies or memory storage for tokens
- All API endpoints require JWT validation (except health/status endpoints)
- Implement rate limiting and CORS properly
- Follow OWASP Top 10 guidelines
- Validate input at API boundary
- Use parameterized queries (Entity Framework handles this)

## Testing Strategy

### Frontend Testing

- **Unit Tests**: Jasmine + Karma for components and services
- **Integration Tests**: Test component interactions and HTTP services
- **E2E Tests**: Playwright/Cypress for user workflows

### Backend Testing

- **Unit Tests**: xUnit for domain entities and application logic
- **Integration Tests**: Test database operations and API endpoints
- **Architecture Tests**: Ensure clean architecture dependencies

### Test Coverage Requirements

- Application layer: >80% coverage
- Domain layer: >80% coverage
- Integration tests for all critical features
- E2E tests for complete user workflows

## Code Style & Quality

### Naming Conventions

- **Files**: kebab-case (`training-card.component.ts`)
- **Classes**: PascalCase (`TrainingCardComponent`)
- **Variables/Methods**: camelCase (`getUserProfile`)
- **Constants**: SCREAMING_SNAKE_CASE (`API_BASE_URL`)
- **Database**: snake_case (`training_sessions`)

### Code Organization

- Follow SOLID principles
- Keep classes/files under 300 lines
- Separate concerns clearly
- Use dependency injection
- Implement proper error handling

## Domain-Specific Guidelines

### SportPlanner Entities

- **Core Entities**: Training, Athlete, Coach, Session, Plan, Subscription
- **Value Objects**: Duration, Intensity, Schedule
- **Aggregates**: Training plans with sessions and progress tracking
- **Events**: Domain events for training lifecycle

### UI/UX Standards

- Follow Material Design guidelines
- Implement responsive design (mobile-first)
- Use 4px grid system for spacing
- Provide loading states and error feedback
- Ensure WCAG 2.1 AA accessibility compliance

## Development Workflow

1. **Quality Gate**: Complete `.clinerules/00-development-gate.md` checklist before any code changes
2. **Context Review**: Read all relevant `.clinerules/` files (see checklist in 00-development-gate.md)
3. **ADR Creation**: Document architectural decisions in `docs/adr/` for significant changes
4. **TDD Approach**: Write tests before implementation
5. **Implementation**: Follow patterns and conventions from .clinerules
6. **Testing**: Run full test suite before commits
7. **Migration**: Update database schema if needed (add migration + update database)
8. **Code Review**: Ensure compliance with all .clinerules

## Important Environment Notes

- **Working Directory**: Commands assume you start from `src/` directory
- **Platform**: Windows environment (paths use backslashes, use `ls` not `dir`)
- **Long-Running Processes**: See `.clinerules/06-tool-usage.md` for guidance on dev servers and watchers