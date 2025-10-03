# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Quality Gate

**CRITICAL: Before any code changes, you MUST read and comply with ALL rules in `.clinerules/` folder.**

Complete the mandatory checklist from [.clinerules/00-development-gate.md](.clinerules/00-development-gate.md) before writing any code. This ensures compliance with architecture, security, testing, and code quality standards.

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
│   ├── src/app/features/        # Feature-based architecture (auth, dashboard, home, shared)
│   ├── src/custom-theme.scss    # Material Design theme
│   └── package.json
├── back/SportPlanner/           # .NET 8 backend
│   ├── src/SportPlanner.Domain/      # Domain entities
│   ├── src/SportPlanner.Application/ # Use cases/commands (MediatR)
│   ├── src/SportPlanner.Infrastructure/ # Data access, EF migrations
│   ├── src/SportPlanner.API/         # Controllers/endpoints
│   └── tests/                        # Unit & integration tests
├── docs/adr/                    # Architecture Decision Records
└── .clinerules/                 # Development standards (MANDATORY READING)
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

# Watch mode build
npm run watch

# Run all tests
npm test

# Run tests with coverage
npm test -- --no-watch --code-coverage

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
- **Feature-Based Structure**: Organize by features (auth, dashboard, training, athlete, coach, planning)
- **Lazy Loading**: Use `loadComponent` for route-level code splitting
- **Signals**: Use Angular signals for reactive state management
- **Material + Tailwind**: Angular Material components with Tailwind utility classes
- **Dependency Injection**: Use Angular's `inject()` function (not constructor injection)

### Backend Architecture

- **Clean Architecture**: Domain → Application → Infrastructure → API layers
- **Vertical Slice**: Organize by features/use cases, not technical concerns
- **CQRS with MediatR**: Commands and queries separated using MediatR
- **Entity Framework**: Direct DbContext usage unless complex domain rules require repositories
- **JWT Authentication**: Supabase JWT validation with automatic JWKS fetching via Authority

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

**CRITICAL**: Backend uses Authority approach in [Program.cs](back/SportPlanner/src/SportPlanner.API/Program.cs) which automatically handles JWKS validation. Do NOT implement custom JWT validation code.

```csharp
// Correct: Authority + Audience (from Program.cs)
options.Authority = $"{builder.Configuration["Supabase:Url"]}/auth/v1";
options.Audience = "authenticated";
```

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

1. **Quality Gate**: Complete [.clinerules/00-development-gate.md](.clinerules/00-development-gate.md) checklist before any code changes
2. **Context Review**: Read all relevant `.clinerules/` files (see checklist in 00-development-gate.md)
3. **ADR Creation**: Document architectural decisions in `docs/adr/` for significant changes (use [template.md](docs/adr/template.md))
4. **TDD Approach**: Write tests before implementation
5. **Implementation**: Follow patterns and conventions from .clinerules
6. **Testing**: Run full test suite before commits
7. **Migration**: Update database schema if needed (add migration + update database)
8. **Code Review**: Ensure compliance with all .clinerules

## Important Environment Notes

- **Working Directory**: Commands assume you start from `src/` directory
- **Platform**: Windows environment (use PowerShell commands, `ls` works)
- **Long-Running Processes**: See [.clinerules/06-tool-usage.md](.clinerules/06-tool-usage.md) for guidance on dev servers and watchers
- **Pre-commit Hooks**: Quality gate validation enforced via `.husky/pre-commit` which runs `.clinerules/validate-quality-gate.js`

## Critical .clinerules References

Before making changes, you MUST review these files based on the type of work:

- **Architecture**: [01-clean-code.md](.clinerules/01-clean-code.md), [14-dotnet-backend.md](.clinerules/14-dotnet-backend.md), [16-design-patterns-csharp.md](.clinerules/16-design-patterns-csharp.md)
- **Frontend**: [10-angular-structure.md](.clinerules/10-angular-structure.md), [11-tailwind.md](.clinerules/11-tailwind.md), [12-material-animations.md](.clinerules/12-material-animations.md), [15-ui-ux-excellence.md](.clinerules/15-ui-ux-excellence.md)
- **Security**: [05-security.md](.clinerules/05-security.md), [13-supabase-jwt.md](.clinerules/13-supabase-jwt.md)
- **Testing**: [04-testing.md](.clinerules/04-testing.md)
- **Conventions**: [02-naming.md](.clinerules/02-naming.md), [03-adr.md](.clinerules/03-adr.md), [06-tool-usage.md](.clinerules/06-tool-usage.md)

## Safe Edit Rules for AI Agents

- Never change `.clinerules/` files or `docs/adr/` without human approval
- When adding migrations: create migration in `SportPlanner.Infrastructure` and update `docs/adr/` if it changes public schema/behavior
- Tests: add unit tests alongside new logic. Backend Application/Domain aim for >80% coverage
- Do not start long-running dev servers in blocking mode
- Avoid editing localization or secrets files without context

## High-Signal Reference Files

- **Frontend entry**: [front/SportPlanner/package.json](front/SportPlanner/package.json)
- **Frontend feature example**: `front/SportPlanner/src/app/features/` (auth, dashboard features)
- **Backend composition root**: [back/SportPlanner/src/SportPlanner.API/Program.cs](back/SportPlanner/src/SportPlanner.API/Program.cs)
- **Infrastructure & EF**: `back/SportPlanner/src/SportPlanner.Infrastructure/`
- **Quality rules**: `.clinerules/` folder
