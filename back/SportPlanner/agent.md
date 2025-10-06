# Backend Agent Instructions - SportPlanner .NET 8 API

> **Complete self-contained guide for .NET 8 Clean Architecture backend**  
> **Context**: Entity Framework Core, PostgreSQL, MediatR CQRS, Supabase JWT

---

## 📑 Table of Contents

1. [Project Structure](#-project-structure)
2. [Quick Commands](#-quick-commands)
3. [Core Principles](#-core-principles)
4. [Clean Architecture Layers](#-clean-architecture-layers)
5. [MediatR CQRS Pattern](#-mediatr-cqrs-pattern)
6. [Entity Framework Core](#-entity-framework-core)
7. [Domain Entities & Value Objects](#-domain-entities--value-objects)
8. [FluentValidation](#-fluentvalidation)
9. [Authentication & JWT](#-authentication--jwt)
10. [Testing Standards](#-testing-standards)
11. [Design Patterns](#-design-patterns)
12. [Error Handling](#-error-handling)
13. [Security Best Practices](#-security-best-practices)
14. [Common Patterns](#-common-patterns)
15. [Pre-Commit Checklist](#-pre-commit-checklist)

---

## 📁 Project Structure

```
back/SportPlanner/
├── src/
│   ├── SportPlanner.Domain/              # Domain layer (entities, value objects, interfaces)
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   ├── Events/
│   │   ├── Enums/
│   │   └── Services/
│   ├── SportPlanner.Application/         # Application layer (use cases, DTOs)
│   │   ├── UseCases/                     # Command/Query handlers (MediatR)
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── DependencyInjection.cs
│   ├── SportPlanner.Infrastructure/      # Infrastructure layer (DB, external services)
│   │   ├── Data/                         # DbContext, configurations
│   │   ├── Repositories/
│   │   ├── ExternalServices/
│   │   ├── Migrations/                   # EF Core migrations
│   │   └── DependencyInjection.cs
│   └── SportPlanner.API/                 # Presentation layer (controllers, middleware)
│       ├── Controllers/
│       ├── Middleware/
│       ├── Configuration/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Program.cs                    # App startup & DI registration
├── tests/
│   ├── SportPlanner.Domain.UnitTests/
│   ├── SportPlanner.Application.UnitTests/
│   ├── SportPlanner.Infrastructure.IntegrationTests/
│   └── SportPlanner.API.IntegrationTests/
└── SportPlanner.sln
```

---

## 🚀 Quick Commands

```bash
# Build & Run
dotnet build                                         # Build entire solution
dotnet test                                          # Run all tests
dotnet run --project src/SportPlanner.API            # Start API (default: https://localhost:7XXX)
dotnet watch --project src/SportPlanner.API          # Start with hot reload

# Entity Framework Migrations (run from back/SportPlanner)
dotnet ef migrations add <MigrationName> --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
dotnet ef database update --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
dotnet ef migrations remove --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API

# Testing
dotnet test --logger "console;verbosity=detailed"    # Verbose test output
dotnet test --collect:"XPlat Code Coverage"          # Generate coverage report
```

---

## ⚡ Core Principles

### 1. **Clean Architecture**

**Dependency Flow**: API → Application → Domain ← Infrastructure

- **Domain**: Core business logic, entities, value objects (NO dependencies)
- **Application**: Use cases, DTOs, interfaces (depends ONLY on Domain)
- **Infrastructure**: Data access, external services (depends on Domain & Application)
- **API**: Controllers, middleware (depends on Application)

**Key Rule**: Inner layers NEVER depend on outer layers.

```
┌─────────────────────────────────────┐
│          API Layer                  │  Controllers, Middleware
├─────────────────────────────────────┤
│       Application Layer             │  Use Cases (MediatR), DTOs, Interfaces
├─────────────────────────────────────┤
│    ┌───────────────────────────┐   │
│    │     Domain Layer          │   │  Entities, Value Objects, Events
│    │   (No Dependencies!)      │   │
│    └───────────────────────────┘   │
│       Infrastructure Layer          │  EF Core, Repositories, External APIs
└─────────────────────────────────────┘
```

### 2. **MediatR CQRS Pattern**

✅ **Commands**: Modify state (Create, Update, Delete)  
✅ **Queries**: Read state (Get, List)

**Benefits**:
- Decouples request/response from handlers
- Easy to test handlers in isolation
- Clear separation of concerns
- Supports cross-cutting concerns (logging, validation) via pipeline behaviors

### 3. **Repository Pattern** (when needed)

Use repositories ONLY for:
- Complex queries with business logic
- Domain invariant enforcement
- Abstracting EF Core details from Application layer

For simple CRUD, use DbContext directly in handlers.

### 4. **Domain-Driven Design (DDD)**

- **Entities**: Objects with identity (e.g., `Training`, `Athlete`)
- **Value Objects**: Objects defined by their attributes (e.g., `Address`, `Email`)
- **Aggregates**: Cluster of entities with root entity (e.g., `Subscription` + `SubscriptionUser`)
- **Domain Events**: Capture business events (e.g., `SubscriptionCreated`)

---

## 🏗️ Clean Architecture Layers

### Domain Layer

**Location**: `SportPlanner.Domain`  
**Dependencies**: NONE  
**Contains**: Entities, Value Objects, Interfaces, Events, Enums

```csharp
// Entity Example
namespace SportPlanner.Domain.Entities;

public class Subscription
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public SubscriptionType Type { get; private set; }
    public Sport Sport { get; private set; }
    public int MaxUsers { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<SubscriptionUser> _users = new();
    public IReadOnlyCollection<SubscriptionUser> Users => _users.AsReadOnly();
    
    // Private constructor for EF Core
    private Subscription() { }
    
    // Public factory method
    public Subscription(Guid ownerId, SubscriptionType type, Sport sport)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("Owner ID cannot be empty", nameof(ownerId));
            
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Type = type;
        Sport = sport;
        MaxUsers = GetMaxUsersByType(type);
        CreatedAt = DateTime.UtcNow;
    }
    
    private static int GetMaxUsersByType(SubscriptionType type)
    {
        return type switch
        {
            SubscriptionType.Basic => 10,
            SubscriptionType.Premium => 50,
            SubscriptionType.Enterprise => 1000,
            _ => throw new ArgumentException($"Unknown subscription type: {type}")
        };
    }
    
    public void AddUser(SubscriptionUser user)
    {
        if (_users.Count >= MaxUsers)
            throw new InvalidOperationException($"Subscription has reached max users ({MaxUsers})");
            
        _users.Add(user);
    }
}
```

```csharp
// Interface in Domain
namespace SportPlanner.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Subscription>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken);
    Task AddAsync(Subscription subscription, CancellationToken cancellationToken);
    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

### Application Layer

**Location**: `SportPlanner.Application`  
**Dependencies**: Domain  
**Contains**: Use Cases (Commands/Queries), DTOs, Validators

```csharp
// Command
namespace SportPlanner.Application.UseCases.Subscriptions.Commands;

public record CreateSubscriptionCommand(SubscriptionType Type, Sport Sport) : IRequest<Guid>;

// Command Handler
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionUserRepository _subscriptionUserRepository;
    private readonly ICurrentUserService _currentUserService;
    
    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionUserRepository subscriptionUserRepository,
        ICurrentUserService currentUserService)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionUserRepository = subscriptionUserRepository;
        _currentUserService = currentUserService;
    }
    
    public async Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.GetUserId();
        var ownerEmail = _currentUserService.GetUserEmail();
        
        // Create subscription (domain logic in entity)
        var subscription = new Subscription(ownerId, request.Type, request.Sport);
        await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        
        // Create subscription user (owner as admin)
        var subscriptionUser = new SubscriptionUser(
            subscription.Id,
            ownerId,
            UserRole.Admin,
            ownerEmail ?? ownerId.ToString()
        );
        await _subscriptionUserRepository.AddAsync(subscriptionUser, cancellationToken);
        
        return subscription.Id;
    }
}
```

```csharp
// Query
namespace SportPlanner.Application.UseCases.Subscriptions.Queries;

public record GetSubscriptionQuery(Guid SubscriptionId) : IRequest<SubscriptionDto>;

// Query Handler
public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, SubscriptionDto>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public GetSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }
    
    public async Task<SubscriptionDto> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        
        if (subscription is null)
            throw new NotFoundException($"Subscription {request.SubscriptionId} not found");
            
        return new SubscriptionDto(
            subscription.Id,
            subscription.OwnerId,
            subscription.Type,
            subscription.Sport,
            subscription.MaxUsers
        );
    }
}
```

### Infrastructure Layer

**Location**: `SportPlanner.Infrastructure`  
**Dependencies**: Domain, Application  
**Contains**: DbContext, Repositories, External Services, Migrations

```csharp
// Repository Implementation
namespace SportPlanner.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SportPlannerDbContext _context;
    
    public SubscriptionRepository(SportPlannerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Subscriptions
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    
    public async Task<List<Subscription>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return await _context.Subscriptions
            .Where(s => s.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        await _context.Subscriptions.AddAsync(subscription, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var subscription = await GetByIdAsync(id, cancellationToken);
        if (subscription is not null)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

### API Layer

**Location**: `SportPlanner.API`  
**Dependencies**: Application  
**Contains**: Controllers, Middleware, Configuration

```csharp
// Controller
namespace SportPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        var command = new CreateSubscriptionCommand(request.Type, request.Sport);
        var subscriptionId = await _mediator.Send(command);
        return Ok(subscriptionId);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription(Guid id)
    {
        var query = new GetSubscriptionQuery(id);
        var subscription = await _mediator.Send(query);
        return Ok(subscription);
    }
}
```

---

## 🎯 MediatR CQRS Pattern

### Commands (Write Operations)

```csharp
// Command: Request to modify state
public record UpdateTrainingCommand(Guid TrainingId, string Name, int DurationMinutes) : IRequest<Unit>;

// Command Handler: Contains business logic
public class UpdateTrainingCommandHandler : IRequestHandler<UpdateTrainingCommand, Unit>
{
    private readonly ITrainingRepository _repository;
    
    public UpdateTrainingCommandHandler(ITrainingRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Unit> Handle(UpdateTrainingCommand request, CancellationToken cancellationToken)
    {
        var training = await _repository.GetByIdAsync(request.TrainingId, cancellationToken);
        
        if (training is null)
            throw new NotFoundException($"Training {request.TrainingId} not found");
            
        // Domain method handles validation
        training.UpdateDetails(request.Name, TimeSpan.FromMinutes(request.DurationMinutes));
        
        await _repository.UpdateAsync(training, cancellationToken);
        
        return Unit.Value;
    }
}
```

### Queries (Read Operations)

```csharp
// Query: Request to retrieve data
public record GetTrainingsByCoachQuery(Guid CoachId) : IRequest<List<TrainingDto>>;

// Query Handler: Read-only operations
public class GetTrainingsByCoachQueryHandler : IRequestHandler<GetTrainingsByCoachQuery, List<TrainingDto>>
{
    private readonly SportPlannerDbContext _context;
    
    public GetTrainingsByCoachQueryHandler(SportPlannerDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TrainingDto>> Handle(GetTrainingsByCoachQuery request, CancellationToken cancellationToken)
    {
        return await _context.Trainings
            .Where(t => t.CoachId == request.CoachId)
            .Select(t => new TrainingDto(t.Id, t.Name, t.Duration, t.ScheduledDate))
            .ToListAsync(cancellationToken);
    }
}
```

### Pipeline Behaviors

```csharp
// Validation Behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();
            
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        
        if (failures.Any())
            throw new ValidationException(failures);
            
        return await next();
    }
}
```

---

## 🗄️ Entity Framework Core

### DbContext Setup

```csharp
public class SportPlannerDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    
    public SportPlannerDbContext(
        DbContextOptions<SportPlannerDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }
    
    public DbSet<Training> Trainings { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionUser> SubscriptionUsers { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Athlete> Athletes { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration implementations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportPlannerDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-populate audit fields for IAuditable entities
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserService?.GetUserId().ToString() ?? "system";
            
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### Entity Configuration

```csharp
public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.OwnerId)
            .IsRequired();
            
        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(s => s.Sport)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(s => s.MaxUsers)
            .IsRequired();
            
        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        // Relationships
        builder.HasMany(s => s.Users)
            .WithOne()
            .HasForeignKey(su => su.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(s => s.OwnerId);
        builder.HasIndex(s => s.CreatedAt);
    }
}
```

### Auditable Interface

```csharp
// Domain Interface
namespace SportPlanner.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
```

---

## 🏛️ Domain Entities & Value Objects

### Entity Pattern

```csharp
namespace SportPlanner.Domain.Entities;

public class Training : IAuditable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public TimeSpan Duration { get; private set; }
    public TrainingIntensity Intensity { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public Guid CoachId { get; private set; }
    public Guid TeamId { get; private set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation properties
    public Team? Team { get; private set; }
    
    // Private constructor for EF Core
    private Training() { }
    
    // Public factory method
    public Training(string name, TimeSpan duration, TrainingIntensity intensity, 
                   DateTime scheduledDate, Guid coachId, Guid teamId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Training name cannot be empty", nameof(name));
        
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive", nameof(duration));
            
        if (scheduledDate <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled date must be in the future", nameof(scheduledDate));
        
        Id = Guid.NewGuid();
        Name = name;
        Duration = duration;
        Intensity = intensity;
        ScheduledDate = scheduledDate;
        CoachId = coachId;
        TeamId = teamId;
    }
    
    // Business methods
    public void UpdateDetails(string name, TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Training name cannot be empty", nameof(name));
        
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive", nameof(duration));
            
        Name = name;
        Duration = duration;
    }
    
    public void Reschedule(DateTime newDate)
    {
        if (newDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot schedule training in the past");
            
        ScheduledDate = newDate;
    }
}
```

### Value Object Pattern

```csharp
namespace SportPlanner.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }
    
    private Email() { }
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));
            
        if (!IsValidEmail(value))
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));
            
        Value = value.ToLowerInvariant();
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);
}

// Base class for Value Objects
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
            
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
}
```

---

## ✅ FluentValidation

### Command Validator

```csharp
public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid subscription type");
            
        RuleFor(x => x.Sport)
            .IsInEnum()
            .WithMessage("Invalid sport type");
    }
}

public class UpdateTrainingCommandValidator : AbstractValidator<UpdateTrainingCommand>
{
    public UpdateTrainingCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Training name is required")
            .MaximumLength(200)
            .WithMessage("Training name cannot exceed 200 characters");
            
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be positive")
            .LessThanOrEqualTo(480)
            .WithMessage("Training cannot exceed 8 hours");
    }
}
```

### Registration in DependencyInjection

```csharp
// In SportPlanner.Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    
    return services;
}
```

---

## 🔐 Authentication & JWT

### Supabase JWT Configuration

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Supabase issues JWTs with JWKS endpoint
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
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
```

### Current User Service

```csharp
public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
    bool IsAuthenticated();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? _httpContextAccessor.HttpContext?.User
            .FindFirst("sub")?.Value;
            
        return Guid.TryParse(userIdClaim, out var userId) 
            ? userId 
            : Guid.Empty;
    }
    
    public string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Email)?.Value 
            ?? _httpContextAccessor.HttpContext?.User
            .FindFirst("email")?.Value 
            ?? string.Empty;
    }
    
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
```

### Controller Authorization

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require JWT for all endpoints
public class TrainingController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TrainingController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> CreateTraining([FromBody] CreateTrainingRequest request)
    {
        var command = new CreateTrainingCommand(/*...*/);
        var trainingId = await _mediator.Send(command);
        return Ok(trainingId);
    }
    
    [HttpGet]
    [AllowAnonymous] // Override [Authorize] for public endpoints
    public async Task<ActionResult<List<TrainingDto>>> GetPublicTrainings()
    {
        // Public endpoint
        return Ok(await _mediator.Send(new GetPublicTrainingsQuery()));
    }
}
```

---

## 🧪 Testing Standards

### Backend Testing Framework
- **Unit Tests**: xUnit + Moq
- **Integration Tests**: xUnit + WebApplicationFactory + Testcontainers
- **Coverage Target**: >80% for Application & Domain layers
- **Test Structure**: AAA pattern (Arrange, Act, Assert)
- **Naming**: `MethodName_Scenario_ExpectedResult`

### Unit Tests (xUnit + Moq)

```csharp
public class CreateSubscriptionCommandHandlerTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionUserRepository> _subscriptionUserRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateSubscriptionCommandHandler _handler;
    
    public CreateSubscriptionCommandHandlerTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _subscriptionUserRepositoryMock = new Mock<ISubscriptionUserRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        var ownerId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.GetUserId()).Returns(ownerId);
        _currentUserServiceMock.Setup(x => x.GetUserEmail()).Returns("owner@example.com");
        
        _handler = new CreateSubscriptionCommandHandler(
            _subscriptionRepositoryMock.Object,
            _subscriptionUserRepositoryMock.Object,
            _currentUserServiceMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ValidCommand_CreatesSubscriptionAndOwnerUser()
    {
        // Arrange
        var command = new CreateSubscriptionCommand(SubscriptionType.Premium, Sport.Football);
        
        // Act
        var subscriptionId = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotEqual(Guid.Empty, subscriptionId);
        _subscriptionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        _subscriptionUserRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<SubscriptionUser>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidSubscriptionType_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateSubscriptionCommand((SubscriptionType)999, Sport.Football);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
```

### Domain Entity Tests

```csharp
public class SubscriptionTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesSubscription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var type = SubscriptionType.Premium;
        var sport = Sport.Football;
        
        // Act
        var subscription = new Subscription(ownerId, type, sport);
        
        // Assert
        Assert.NotEqual(Guid.Empty, subscription.Id);
        Assert.Equal(ownerId, subscription.OwnerId);
        Assert.Equal(type, subscription.Type);
        Assert.Equal(sport, subscription.Sport);
        Assert.Equal(50, subscription.MaxUsers); // Premium = 50
    }
    
    [Fact]
    public void Constructor_EmptyOwnerId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Subscription(Guid.Empty, SubscriptionType.Basic, Sport.Football));
    }
    
    [Fact]
    public void AddUser_BelowMaxUsers_AddsUserSuccessfully()
    {
        // Arrange
        var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Basic, Sport.Football);
        var user = new SubscriptionUser(subscription.Id, Guid.NewGuid(), UserRole.Member, "user@example.com");
        
        // Act
        subscription.AddUser(user);
        
        // Assert
        Assert.Contains(user, subscription.Users);
    }
    
    [Fact]
    public void AddUser_ExceedsMaxUsers_ThrowsInvalidOperationException()
    {
        // Arrange
        var subscription = new Subscription(Guid.NewGuid(), SubscriptionType.Basic, Sport.Football); // Max 10
        for (int i = 0; i < 10; i++)
        {
            subscription.AddUser(new SubscriptionUser(subscription.Id, Guid.NewGuid(), UserRole.Member, $"user{i}@example.com"));
        }
        
        var extraUser = new SubscriptionUser(subscription.Id, Guid.NewGuid(), UserRole.Member, "extra@example.com");
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => subscription.AddUser(extraUser));
    }
}
```

### Integration Tests

```csharp
public class SubscriptionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public SubscriptionControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateSubscription_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new CreateSubscriptionRequest
        {
            Type = SubscriptionType.Premium,
            Sport = Sport.Football
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/subscription", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var subscriptionId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, subscriptionId);
    }
}
```

---

## 🎨 Design Patterns

### When to Use Design Patterns

**Ask yourself**: What problem am I solving?

- **Creating objects flexibly?** → Creational Patterns
- **Organizing classes/objects?** → Structural Patterns
- **Managing algorithms/communication?** → Behavioral Patterns

### Creational Patterns

**Factory Method**
- **When**: Don't know exact type of object to create upfront
- **Use Case**: Document generator (PDF, DOCX, TXT) - subclasses implement `CreateDocument()`

**Builder**
- **When**: Constructing complex objects step by step
- **Use Case**: `UserConfigurationBuilder` for object with many optional properties

**Singleton**
- **When**: Need single instance with global access
- **⚠️ CAUTION**: Use sparingly, can introduce global coupling
- **Use Case**: `AppSettings`, logging service

### Structural Patterns

**Adapter**
- **When**: Make incompatible interfaces work together
- **Use Case**: `XmlToJsonAdapter` for external library returning XML

**Decorator**
- **When**: Add behavior dynamically without modifying original
- **Use Case**: Notification system - `SmsNotifierDecorator`, `PushNotifierDecorator`

**Facade**
- **When**: Simplify complex subsystem with single interface
- **Use Case**: `PaymentFacade` orchestrating multiple payment services

### Behavioral Patterns

**Strategy**
- **When**: Multiple algorithms for same task, chosen at runtime
- **Use Case**: Compression strategies - `ZipCompressionStrategy`, `RarCompressionStrategy`

**Observer**
- **When**: Notify multiple objects about events
- **Use Case**: Order state changes notify `EmailService`, `InventoryService`, `AnalyticsService`

**Command**
- **When**: Encapsulate requests as objects (undo/redo, queuing)
- **Use Case**: Undo/redo functionality - each action is a `Command` with `Undo()` method

**State**
- **When**: Object behavior changes based on internal state
- **Use Case**: Document workflow - `DraftState`, `ReviewState`, `PublishedState`

---

## ⚠️ Error Handling

### Custom Exceptions

```csharp
namespace SportPlanner.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation errors occurred")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
```

### Global Exception Middleware

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, notFound.Message),
            ValidationException validation => (StatusCodes.Status400BadRequest, "Validation failed"),
            ForbiddenException forbidden => (StatusCodes.Status403Forbidden, forbidden.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An internal error occurred")
        };
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var response = new
        {
            statusCode,
            message,
            errors = exception is ValidationException validationEx ? validationEx.Errors : null
        };
        
        return context.Response.WriteAsJsonAsync(response);
    }
}

// Register in Program.cs
app.UseMiddleware<GlobalExceptionMiddleware>();
```

---

## 🛡️ Security Best Practices

### Input Validation
- **Always validate** user inputs with FluentValidation
- **Sanitize** data before database operations
- **Use parameterized queries** (EF Core does this automatically)

### Authentication & Authorization
- **JWT tokens** from Supabase for authentication
- **[Authorize]** attribute on all controllers by default
- **[AllowAnonymous]** only for truly public endpoints
- **Claims-based authorization** for role/permission checks

### Secrets Management
- **Never commit** secrets in appsettings.json
- Use **User Secrets** for local development (`dotnet user-secrets set`)
- Use **Environment Variables** or **Azure Key Vault** in production

```bash
# Local development
dotnet user-secrets init --project src/SportPlanner.API
dotnet user-secrets set "Supabase:Authority" "https://xxx.supabase.co/auth/v1" --project src/SportPlanner.API
```

### SQL Injection Prevention
- ✅ **EF Core parameterizes** queries automatically
- ❌ **Avoid raw SQL** unless absolutely necessary
- ✅ If raw SQL needed, use `FromSqlRaw` with parameters:

```csharp
var userId = Guid.NewGuid();
var trainings = await _context.Trainings
    .FromSqlRaw("SELECT * FROM Trainings WHERE CoachId = {0}", userId)
    .ToListAsync();
```

### HTTPS Only
```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

---

## 🎯 Common Patterns

### Result Pattern (for Railway-Oriented Programming)

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// Usage in handler
public async Task<Result<Guid>> Handle(CreateTrainingCommand request, CancellationToken cancellationToken)
{
    try
    {
        var training = new Training(/*...*/);
        await _repository.AddAsync(training, cancellationToken);
        return Result<Guid>.Success(training.Id);
    }
    catch (Exception ex)
    {
        return Result<Guid>.Failure(ex.Message);
    }
}
```

### Specification Pattern (for complex queries)

```csharp
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }
}

public class ActiveTrainingsSpecification : Specification<Training>
{
    public override Expression<Func<Training, bool>> ToExpression()
    {
        return training => training.ScheduledDate > DateTime.UtcNow;
    }
}

// Usage
var activeTrainings = await _context.Trainings
    .Where(new ActiveTrainingsSpecification().ToExpression())
    .ToListAsync();
```

---

## ✅ Pre-Commit Checklist

Before committing code, verify:

- [ ] All layers follow dependency rules (no circular dependencies)
- [ ] Commands/Queries use MediatR pattern correctly
- [ ] Entity constructors are private (for EF Core)
- [ ] Business logic is in Domain entities, not handlers
- [ ] FluentValidation added for all commands
- [ ] Tests added/updated for new features (>80% coverage)
- [ ] No secrets in appsettings.json (use User Secrets)
- [ ] Controllers use `[Authorize]` by default
- [ ] Exception handling via global middleware
- [ ] Migrations generated for DB changes
- [ ] No `Console.WriteLine` (use ILogger)
- [ ] API endpoints return proper HTTP status codes
- [ ] DTOs used for API responses (not entities)
- [ ] Repository pattern used ONLY when needed
- [ ] Async/await used correctly (no `.Result` or `.Wait()`)

---

## 🚫 Common Mistakes to Avoid

❌ Returning domain entities from controllers (use DTOs)  
❌ Business logic in controllers or handlers (put in Domain)  
❌ Using `.Result` or `.Wait()` (use async/await)  
❌ Circular dependencies between layers  
❌ Hardcoded connection strings or secrets  
❌ Missing `[Authorize]` attribute on controllers  
❌ Not using CancellationToken in async methods  
❌ Large command/query handlers (split into smaller handlers)  
❌ Not validating commands with FluentValidation  
❌ Exposing infrastructure details in Application layer  
❌ Using raw SQL instead of EF Core LINQ  
❌ Not following AAA pattern in tests  
❌ Missing error handling middleware  
❌ Not using dependency injection properly  

---

## 📚 Key Files to Reference

- **Program.cs**: `src/SportPlanner.API/Program.cs` (startup, DI, middleware)
- **DbContext**: `src/SportPlanner.Infrastructure/Data/SportPlannerDbContext.cs`
- **DI Registration**: 
  - `src/SportPlanner.Application/DependencyInjection.cs`
  - `src/SportPlanner.Infrastructure/DependencyInjection.cs`
- **appsettings**: `src/SportPlanner.API/appsettings.json` (DO NOT COMMIT SECRETS)
- **General Agent**: `../agent.md` (cross-cutting concerns, security, naming)
- **Frontend Agent**: `../front/agent.md` (API contracts, DTOs)

---

**Last Updated**: 2025-10-06  
**Version**: 2.0 (Self-contained, no external dependencies)
