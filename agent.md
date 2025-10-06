# SportPlannerV2 - AI Agent Instructions

> **Project Overview**: Full-stack sports planning and management application with Angular 20 frontend and .NET 8 backend.

---

## 🎯 Quick Start

**Before making ANY changes:**
1. Complete the **MANDATORY Quality Gate Checklist** below
2. Review relevant sections:
   - **Frontend work**: See `front/agent.md`
   - **Backend work**: See `back/agent.md`
3. Check `docs/adr/` for architectural decisions
4. **NEVER** modify `docs/adr/` without human approval

---

## 📋 What This File Contains

This `agent.md` contains **cross-cutting concerns** for the entire project:
- Architecture decisions (ADRs)
- Security standards (authen## 📚 Additional Resources

- **Detailed Frontend Instructions**: See `front/agent.md` (Angular, Tailwind, Signals, components)
- **Detailed Backend Instructions**: See `back/agent.md` (Clean Architecture, MediatR, EF Core, testing)
- **Tailwind Component Library**: See `NO_ANGULAR_MATERIAL.md` (ready-to-use Tailwind patterns)
- **ADR Template**: See template in "Architecture Decisions" section above
- **Training System Spec**: See `docs/training-system-complete-specification.md`, secrets, HTTPS)
- Naming conventions (files, classes, variables)
- Quality gate checklist
- Where to find things

**For specific technology guidance:**
- 👉 **Frontend (Angular/Tailwind)**: See `front/agent.md`
- 👉 **Backend (.NET/EF Core)**: See `back/agent.md`---

## 🛡️ Security Standards

### Authentication & Authorization
- **JWT Validation**: ALWAYS validate JWT via JWKS from Supabase (backend automatically via Authority)
- **Token Storage**: NEVER store tokens or passwords in localStorage/sessionStorage
- **Session Management**: Use httpOnly cookies for sensitive tokens when possible
- **Refresh Tokens**: Implement automatic token rotation
- **Password Requirements**: Enforce strong passwords (min 8 chars, mixed case, numbers, symbols)

### Input Validation
- **Client-side**: Validation for UX ONLY, NEVER for security
- **Server-side**: Strict validation of ALL inputs (required, type, format, range)
- **Sanitization**: Clean data before processing or storing
- **SQL Injection**: ALWAYS use prepared parameters/parameterized queries, NEVER string concatenation
- **XSS Prevention**: Angular sanitizes by default, avoid `bypassSecurityTrust*` unless absolutely necessary

**Backend Validation Example:**
```csharp
public class CreateTrainingCommandValidator : AbstractValidator<CreateTrainingCommand>
{
    public CreateTrainingCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name too long");
            
        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be positive")
            .LessThan(1440).WithMessage("Duration cannot exceed 24 hours");
    }
}
```

### Secrets Management
- **Environment Variables**: ALL secrets MUST be in environment variables
- **Never Commit**: NEVER commit secrets, API keys, or passwords to repository
- **Rotation**: Rotate keys following best practices (every 90 days minimum)
- **Access Control**: Principle of least privilege
- **.gitignore**: Ensure `appsettings.json`, `environment.ts` with secrets are ignored

**Checking for secrets before commit:**
```bash
# Run before committing
git diff --cached | grep -iE "password|secret|api_key|private_key|token"
```

**Environment Variables Structure:**
```bash
# Backend (.NET)
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_ANON_KEY=your-anon-key
SUPABASE_SERVICE_KEY=your-service-key  # Server-side only
DATABASE_CONNECTION=your-connection-string

# Frontend (Angular)
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_ANON_KEY=your-anon-key  # Public key, safe for client
```

### HTTPS and Communication
- **TLS**: FORCE HTTPS in production (redirect HTTP to HTTPS)
- **CORS**: Configure specific domains, NEVER use wildcard (`*`) in production
- **Security Headers**: Implement essential headers:
  - `Content-Security-Policy`: Prevent XSS attacks
  - `Strict-Transport-Security`: Force HTTPS
  - `X-Content-Type-Options: nosniff`: Prevent MIME sniffing
  - `X-Frame-Options: DENY`: Prevent clickjacking
- **API Security**: Rate limiting and throttling to prevent abuse

**Backend Security Headers (.NET):**
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
    await next();
});
```

### SportPlanner Specific Security
- **Health Data**: Encrypt at rest and in transit (contains sensitive athlete health info)
- **Personal Information**: GDPR/LOPD compliance required
- **Training Data**: Anonymize for analytics and reporting
- **Device Data**: Validate integrity of sensor/wearable data

### Auditing and Logging
- **Access Logs**: Record access to sensitive data (health, personal info)
- **Failed Attempts**: Log failed authentication attempts
- **Data Changes**: Audit trail for critical data modifications
- **Security Events**: Automatic alerts for suspicious activity

**Audit Example (IAuditable interface):**
```csharp
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}

// Auto-populated in DbContext.SaveChangesAsync
```

### OWASP Top 10 Compliance
- ✅ **Injection**: Parameterized queries, input validation
- ✅ **Broken Authentication**: Supabase JWT, strong passwords, MFA support
- ✅ **Sensitive Data Exposure**: Encryption, secure storage, HTTPS only
- ✅ **XML External Entities (XXE)**: Validate XML inputs
- ✅ **Broken Access Control**: Role-based authorization, ownership checks
- ✅ **Security Misconfiguration**: Hardened infrastructure, minimal permissions
- ✅ **Cross-Site Scripting (XSS)**: Output sanitization, CSP headers
- ✅ **Insecure Deserialization**: Validate deserialized data
- ✅ **Using Components with Known Vulnerabilities**: Regular dependency updates
- ✅ **Insufficient Logging & Monitoring**: Comprehensive logging, alerting

---

## 📂 Where to Find Things

### Frontend (`front/SportPlanner/src/app/`)* modify `docs/adr/` without human approval

---

## 📁 Project Structure

```
src/
├── front/SportPlanner/          # Angular 20 frontend (Standalone components)
├── back/SportPlanner/           # .NET 8 backend (Clean Architecture)
├── docs/                        # Architecture Decision Records (ADRs)
├── .github/                     # CI/CD and copilot instructions
├── agent.md                     # This file (general instructions)
├── front/agent.md              # Frontend-specific instructions
└── back/agent.md               # Backend-specific instructions
```

---

## 🛠️ Technology Stack

### Frontend
- **Framework**: Angular 20 (standalone components, no NgModules)
- **Styling**: Tailwind CSS v4 **ONLY** (❌ NO Angular Material)
- **Icons**: Heroicons or Lucide (❌ NO Material Icons)
- **State**: Angular Signals (mandatory for reactive state)
- **Auth**: Supabase client (`@supabase/supabase-js`)
- **Build**: esbuild via Angular CLI

### Backend
- **Framework**: .NET 8 (C#)
- **Architecture**: Clean Architecture (Domain → Application → Infrastructure → API)
- **Patterns**: MediatR (CQRS), Repository Pattern (when needed)
- **Database**: PostgreSQL via Entity Framework Core
- **Auth**: Supabase JWT (Authority/JWKS validation)

### Infrastructure
- **Auth Provider**: Supabase (JWT tokens, JWKS validation)
- **Database**: PostgreSQL (Supabase-hosted)
- **Version Control**: Git (GitHub)

---

## ⚡ Common Commands

**Run from `src/` directory:**

### Frontend Development
```bash
cd front/SportPlanner
npm start              # Dev server (localhost:4200)
npm test               # Run unit tests
npm run build          # Production build
npm run watch          # Build in watch mode
```

### Backend Development
```bash
cd back/SportPlanner
dotnet build                                    # Build solution
dotnet test                                     # Run all tests
dotnet run --project src/SportPlanner.API       # Start API
```

### Database Migrations (EF Core)
```bash
cd back/SportPlanner
dotnet ef migrations add <MigrationName> --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
dotnet ef database update --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
```

---

## 🚨 Critical Rules

### 1. **MANDATORY: Pre-Development Quality Gate**

**BEFORE writing a single line of code**, complete this checklist:

#### Architecture Understanding
- [ ] Understand frontend structure (standalone components, Signals, Tailwind)
- [ ] Understand backend structure (Clean Architecture: Domain → Application → Infrastructure → API)
- [ ] Know where your changes fit in the architecture
- [ ] Reviewed existing similar features for patterns

#### Security & Authentication
- [ ] Confirm Supabase JWT validation (NO custom JWT)
- [ ] Verify issuer/audience configuration
- [ ] Understand token validation flow (Authority/JWKS)
- [ ] Know secrets management (environment variables only)
- [ ] NO tokens in localStorage/sessionStorage

#### Code Quality Standards
- [ ] Follow naming conventions (see below)
- [ ] Apply SOLID principles
- [ ] Keep functions small and focused (<20 lines)
- [ ] No code duplication (DRY principle)
- [ ] Files under 300 lines

#### Testing Requirements
- [ ] Plan tests upfront (Unit, Integration, E2E)
- [ ] Target >80% coverage for Application/Domain layers
- [ ] Use AAA pattern (Arrange, Act, Assert)
- [ ] Mock external dependencies (Supabase, HTTP, DB)

#### UI/UX Standards (Frontend)
- [ ] **NO Angular Material** - Use Tailwind CSS only
- [ ] Component communication patterns understood
- [ ] Responsive design (mobile-first)
- [ ] Accessibility considerations (ARIA labels, keyboard nav)

#### Decision Tree: Can I Start Coding?

**If minor change** (bugfix, typo, internal refactor):
- [ ] Doesn't affect architecture → Yes, proceed
- [ ] Doesn't affect security → Yes, proceed
- [ ] Doesn't affect UI/UX patterns → Yes, proceed

**If new feature or significant change**:
- [ ] Completed checklist above → Yes
- [ ] Requires ADR? (see below) → Create ADR first
- [ ] Affects multiple layers? → Consider pair programming
- [ ] Then proceed with TDD: Tests → Code → Refactor

### 2. **Architecture Decisions (ADRs)**

If your change involves:
- New architectural pattern or technology
- Security or authentication changes
- Public API modifications
- Major refactoring
- Database schema changes
- Integration with external services

**→ Create an ADR** in `docs/adr/` using this template:

```markdown
# ADR-XXX: Title of Decision

**Date**: YYYY-MM-DD
**Status**: [Proposed|Accepted|Rejected|Deprecated|Superseded by ADR-YYY]
**Context**: Sprint/Feature/Issue related

## Context
Description of the problem or situation requiring a decision.

## Options Considered
1. **Option A**: Description, pros, cons
2. **Option B**: Description, pros, cons
3. **Option C**: Description, pros, cons

## Decision
Chosen option and justification.

## Consequences
### Positive
- Benefit 1
- Benefit 2

### Negative
- Risk/limitation 1
- Risk/limitation 2

## Implementation Notes
- Technical details
- Migration path if needed

## References
- Issue: #123
- PR: #456
- Related ADRs: ADR-YYY
```

### 3. **Authentication (Supabase JWT)**

✅ **ALWAYS** use Supabase JWT with Authority/JWKS validation  
❌ **NEVER** implement custom JWT logic  
❌ **NEVER** hardcode secrets or tokens  
✅ Use environment variables for all sensitive config

**Backend JWT Configuration (.NET):**
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Authority automatically discovers JWKS endpoint
        options.Authority = builder.Configuration["Supabase:Authority"];
        // Example: "https://<project-id>.supabase.co/auth/v1"
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Supabase:Authority"],
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // JWKS endpoint auto-discovered from Authority
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
```

**What this does automatically:**
- ✅ Downloads JWKS from `/.well-known/jwks.json`
- ✅ Caches keys with appropriate duration
- ✅ Handles automatic key rotation
- ✅ Industry standard OAuth2/OIDC compliance

**Frontend JWT Usage (Angular):**
```typescript
// auth.service.ts
export class AuthService {
  private supabase = createClient(
    environment.supabaseUrl,
    environment.supabaseAnonKey
  );
  
  async signIn(email: string, password: string) {
    const { data, error } = await this.supabase.auth.signInWithPassword({
      email,
      password
    });
    return { data, error };
  }
  
  async getSession() {
    return await this.supabase.auth.getSession();
  }
  
  getAccessToken(): string | null {
    // Get from Supabase session, NOT from localStorage
    return this.supabase.auth.session()?.access_token ?? null;
  }
}
```

**Role-Based Access Control:**
```typescript
export enum UserRole {
  ATHLETE = 'athlete',
  COACH = 'coach',
  ADMIN = 'admin',
  NUTRITIONIST = 'nutritionist'
}

interface UserClaims {
  sub: string;
  email: string;
  role: UserRole;
  permissions: string[];
}
```

### 4. **Testing Standards**

**Coverage Requirements:**
- Unit tests: >80% coverage in Application/Domain layers
- Integration tests: For critical features and APIs
- E2E tests: For complete user flows
- Component tests: For complex Angular components

**Test Structure:**
```
tests/
├── SportPlanner.Domain.UnitTests/
├── SportPlanner.Application.UnitTests/
├── SportPlanner.Infrastructure.IntegrationTests/
└── SportPlanner.API.IntegrationTests/
```

**AAA Pattern (Arrange, Act, Assert):**
```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateSubscription()
{
    // Arrange
    var command = new CreateSubscriptionCommand(
        SubscriptionType.Team, 
        Sport.Football
    );
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.NotEqual(Guid.Empty, result);
    _subscriptionRepositoryMock.Verify(
        x => x.AddAsync(
            It.IsAny<Subscription>(), 
            It.IsAny<CancellationToken>()
        ), 
        Times.Once
    );
}
```

**Mocking Strategy:**
- **Supabase**: Mock client and CRUD operations
- **HTTP**: Mock HttpClient calls
- **Database**: Use in-memory DB or mocks
- **External Services**: Mock all third-party APIs

### 5. **Code Quality (SOLID, DRY, Clean Code)**

**SOLID Principles:**
- **Single Responsibility**: Each class/function has ONE reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived objects substitutable for base classes
- **Interface Segregation**: Clients don't depend on unused interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

**DRY (Don't Repeat Yourself):**
- Extract duplicated code to functions/services
- Use constants for repeated values
- Create shared services for common logic

**Clean Code Rules:**
- Keep classes and files short (<300 lines)
- Don't mix UI and business logic
- Clear separation: presentation, business logic, data access
- Self-documenting code with descriptive names
- Comments only when "why" isn't obvious
- Small functions (<20 lines) with clear purpose
- Avoid excessive nesting (max 3 levels)
- Early returns for guard clauses

**Error Handling:**
- Explicit error handling, don't ignore errors
- Use specific error types when possible
- Informative logs for debugging
- User-friendly error messages

### 6. **UI Components (Tailwind Only - NO Angular Material)**

✅ **ALWAYS** use Tailwind CSS for all UI  
❌ **NEVER** import or use Angular Material modules  
❌ **NEVER** use Material Icons  
✅ Use Heroicons or Lucide for icons

**See `front/agent.md` for complete Tailwind component library**

---

## � Naming Conventions

### Files and Folders
- **Files**: kebab-case (`user-profile.component.ts`)
- **Folders**: kebab-case (`user-management/`)
- **Angular Components**: `feature-name.component.ts`
- **Angular Services**: `feature-name.service.ts`
- **Models**: `feature-name.model.ts`
- **Tests**: `feature-name.spec.ts` or `feature-name.test.ts`

### Classes and Interfaces
- **Components**: PascalCase + Component suffix (`UserProfileComponent`)
- **Services**: PascalCase + Service suffix (`UserManagementService`)
- **Interfaces**: PascalCase with optional I prefix (`User` or `IUser`)
- **Types**: PascalCase (`UserRole`, `TrainingStatus`)
- **Enums**: PascalCase (`UserStatus`, `SubscriptionType`)

### Variables and Functions
- **Variables**: camelCase (`userName`, `isAuthenticated`, `hasAccess`)
- **Functions/Methods**: camelCase with initial verb (`getUserProfile`, `validateForm`, `calculateTotal`)
- **Const primitives**: SCREAMING_SNAKE_CASE (`API_BASE_URL`, `MAX_RETRY_ATTEMPTS`)
- **Const objects**: camelCase (`defaultConfig`, `routePaths`)
- **Private properties**: underscore prefix `_` (`_internalState`, `_cache`)
- **Boolean variables**: is/has/can prefix (`isLoading`, `hasPermission`, `canEdit`)

### C# Specific (Backend)
- **Classes**: PascalCase (`TrainingService`, `SubscriptionRepository`)
- **Methods**: PascalCase (`GetUserById`, `CreateTraining`)
- **Properties**: PascalCase (`UserName`, `CreatedAt`)
- **Private fields**: camelCase with underscore (`_userRepository`, `_logger`)
- **Interfaces**: PascalCase with I prefix (`IUserRepository`, `ITrainingService`)
- **Constants**: PascalCase (`MaxRetryAttempts`, `DefaultTimeout`)

### Database
- **Tables**: snake_case (`training_sessions`, `athlete_profiles`, `subscription_users`)
- **Columns**: snake_case (`created_at`, `user_id`, `is_active`)
- **Indexes**: descriptive format (`idx_training_sessions_athlete_id`)
- **Foreign Keys**: `fk_table_referenced_table` (`fk_trainings_users`)

### URLs and Routes
- **Routes**: kebab-case (`/training-plans`, `/athlete-profile`, `/subscription-management`)
- **Query params**: camelCase (`?athleteId=123&startDate=2024-01-01`)
- **API endpoints**: kebab-case with version (`/api/v1/training-plans`, `/api/v1/subscriptions`)

### SportPlanner Domain Specific
- **Entities**: `Training`, `Athlete`, `Coach`, `Session`, `Plan`, `Subscription`, `SubscriptionUser`
- **Services**: `TrainingService`, `AthleteService`, `PlanningService`, `SubscriptionService`
- **States**: `TrainingState`, `AthleteProgress`, `SessionStatus`, `SubscriptionStatus`
- **Events**: `TrainingCreated`, `SessionCompleted`, `PlanUpdated`, `SubscriptionPurchased`
- **DTOs**: `CreateTrainingDto`, `UpdateAthleteDto`, `SubscriptionResponseDto`
- **Commands**: `CreateTrainingCommand`, `UpdateSessionCommand`
- **Queries**: `GetTrainingByIdQuery`, `ListActiveAthletesQuery`

---

## �📂 Where to Find Things

### Frontend (`front/SportPlanner/src/app/`)
- **Features**: `features/` (training, teams, athletes, etc.)
- **Shared Components**: `shared/components/`
- **Services**: `core/services/` or feature-specific `services/`
- **Models**: `core/models/` or feature-specific `models/`
- **Auth**: `core/auth/` (guards, interceptors, services)
- **Routing**: Feature-based routes in `*.routes.ts` files
- **Notifications**: `shared/notifications/` (global notification system)

### Backend (`back/SportPlanner/src/`)
- **API Controllers**: `SportPlanner.API/Controllers/`
- **Use Cases (Commands/Queries)**: `SportPlanner.Application/UseCases/`
- **Domain Entities**: `SportPlanner.Domain/Entities/`
- **Repositories**: `SportPlanner.Infrastructure/Repositories/`
- **EF Migrations**: `SportPlanner.Infrastructure/Migrations/`
- **Auth Config**: `SportPlanner.API/Program.cs` (JWT setup)

### Documentation
- **ADRs**: `docs/adr/` (architectural decisions)
- **API Docs**: `docs/training-system-complete-specification.md`
- **Rules**: `agent.md`, `front/agent.md`, `back/agent.md` (quality rules)

---

## 🎨 Conventions

### Naming
- **C# (Backend)**: PascalCase for classes/methods, camelCase for locals
- **TypeScript (Frontend)**: PascalCase for classes/components, camelCase for functions/vars
- **Files**: kebab-case for Angular files, PascalCase for C# files
- **Routes**: kebab-case (e.g., `/training-plans`, `/user-profile`)

### Code Organization
- **Frontend**: Feature-based structure (`features/training/`, `features/teams/`)
- **Backend**: Clean Architecture layers (Domain, Application, Infrastructure, API)
- **Tests**: Mirror source structure (`UseCases/CreateSubscriptionCommandTests.cs`)

### State Management (Frontend)
- **Local State**: Angular Signals (`signal<T>()`)
- **Shared State**: Services with Signals
- **Async Operations**: RxJS for complex streams, otherwise Promises
- ❌ **Avoid**: BehaviorSubject/Subject for primary state (use Signals instead)

---

## 🚫 What NOT to Touch

### Ignore Patterns (for file operations)
```
**/node_modules/**
**/bin/**
**/obj/**
**/.angular/**
**/dist/**
**/.vscode/**
**/.git/**
```

### Protected Areas (require human approval)
- `agent.md`, `front/agent.md`, `back/agent.md` - Quality rules
- `docs/adr/**` - Architecture decisions
- `.husky/**` - Git hooks
- `.github/workflows/**` - CI/CD pipelines
- `**/appsettings.json` - Sensitive config (use Development/env files)

---

## 🔍 High-Signal Files (start here for context)

### Understanding the System
1. **Frontend Entry**: `front/SportPlanner/src/main.ts`, `front/SportPlanner/src/app/app.component.ts`
2. **Backend Entry**: `back/SportPlanner/src/SportPlanner.API/Program.cs`
3. **Auth Setup**: `back/SportPlanner/src/SportPlanner.API/Program.cs` (JWT config)
4. **Database Context**: `back/SportPlanner/src/SportPlanner.Infrastructure/Data/SportPlannerDbContext.cs`

### Feature Examples
- **Training System**: `front/SportPlanner/src/app/features/training/`
- **Subscription System**: `back/SportPlanner/src/SportPlanner.Application/UseCases/CreateSubscriptionCommandHandler.cs`
- **Notifications**: `front/SportPlanner/src/app/shared/notifications/`

---

## 🧪 Pre-Commit Quality Gate

A pre-commit hook can be configured to ensure:
- ✅ All quality rules are followed
- ✅ Tests pass
- ✅ No console.logs or debugger statements
- ✅ Code is properly formatted
- ✅ No hardcoded secrets detected

**Recommended Pre-Commit Checks:**
```bash
#!/bin/bash
# .husky/pre-commit or similar

# Run tests
npm test --passWithNoTests
dotnet test

# Check for console.log/debugger
git diff --cached | grep -E "console\\.log|debugger" && echo "Remove console.log/debugger" && exit 1

# Check for secrets
git diff --cached | grep -iE "password|secret|api_key" && echo "Possible secret detected" && exit 1

# Format check
npm run lint
dotnet format --verify-no-changes
```

**Fix failures before committing.**

---

## 💡 Best Practices

### When Adding a Feature
1. **Plan**: Identify affected layers (frontend feature, backend use-case, domain entity)
2. **Review This Guide**: Check relevant sections (testing, naming, security, etc.)
3. **Write Tests First**: TDD approach when possible
4. **Implement**: Follow patterns from existing features
5. **Document**: Add ADR if architectural impact (see template above)
6. **Verify**: Run tests, build, check for errors

### When Debugging
1. Check recent changes in git history
2. Review test failures for clues
3. Check browser console (frontend) or logs (backend)
4. Verify environment variables and config
5. Use breakpoints, not `console.log`

### When Refactoring
1. Ensure tests exist and pass
2. Refactor incrementally
3. Run tests after each change
4. Update documentation if public API changes

---

## 📚 Additional Resources

- **Detailed Frontend Instructions**: See `front/agent.md` (Angular, Signals, Tailwind components)
- **Detailed Backend Instructions**: See `back/agent.md` (Clean Architecture, MediatR, EF Core)
- **Tailwind Component Library**: See `front/agent.md` (ready-to-use patterns)
- **ADR Template**: See template in "Architecture Decisions" section above
- **Training System Spec**: See `docs/training-system-complete-specification.md`
- **NO Angular Material Guide**: See `NO_ANGULAR_MATERIAL.md` for migration info

---

## 🤝 Working with AI Agents

### Communication Style
- Be specific about what changed and why
- Ask for clarification when requirements are unclear
- Suggest alternatives when constraints block the ideal solution

### File Operations
- Read relevant context before editing
- Make focused, incremental changes
- Verify builds and tests after changes
- Avoid modifying generated files (migrations, compiled code)

### When Blocked
- Document the blocker clearly
- Propose workarounds
- Request human review for protected areas
- Don't assume—verify with code/docs

---

## 🚫 Files to Ignore (for AI agents)

When performing file operations, ALWAYS ignore:
```
**/node_modules/**
**/bin/**
**/obj/**
**/.angular/**
**/dist/**
**/.vscode/**
**/.git/**
**/package-lock.json
**/yarn.lock
**/.env
**/.env.local
```

---

**Last Updated**: 2025-10-06  
**Status**: ✅ Self-contained - No external dependencies  
**Version**: 2.0 (Consolidated)
