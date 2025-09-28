# .NET 8 Backend Architecture

## Clean Architecture Structure
Implementar arquitectura limpia con separación clara de responsabilidades:

```
src/
├── SportPlanner.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Interfaces/
│   └── Events/
├── SportPlanner.Application/
│   ├── UseCases/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── SportPlanner.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   ├── ExternalServices/
│   └── Configuration/
└── SportPlanner.API/
    ├── Controllers/
    ├── Middleware/
    ├── Configuration/
    └── Program.cs
```

## Entity Framework Configuration
- **DbContext**: Uso directo salvo para invariantes de dominio
- **Repository Pattern**: Solo cuando hay reglas de negocio complejas
- **Migrations**: Versionadas y con rollback strategy
- **Connection Strings**: Por ambiente via Configuration

```csharp
// DbContext Example
public class SportPlannerDbContext : DbContext
{
    public SportPlannerDbContext(DbContextOptions<SportPlannerDbContext> options)
        : base(options) { }

    public DbSet<Training> Trainings { get; set; }
    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<Coach> Coaches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportPlannerDbContext).Assembly);
    }
}
```

## Auditing Entities (IAuditable)
For tracking creation and modification of entities, we will use an `IAuditable` interface and automate the process in the `DbContext`.

### 1. IAuditable Interface
This interface will be defined in the `SportPlanner.Domain` project.

```csharp
// In: SportPlanner.Domain/Interfaces/IAuditable.cs
public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } // Or Guid
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
```

### 2. DbContext Implementation
The `SportPlannerDbContext` will be modified to automatically fill in the audit properties before saving changes to the database. This requires injecting `ICurrentUserService` to identify the user.

```csharp
// In: SportPlanner.Infrastructure/Data/SportPlannerDbContext.cs
public class SportPlannerDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public SportPlannerDbContext(
        DbContextOptions<SportPlannerDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // ... DbSets

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.GetUserId();
        var now = DateTime.UtcNow;

        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentUserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    
    // ... OnModelCreating
}
```
This approach centralizes the audit logic, keeping the application's use cases clean.


## JWT Middleware Configuration
- **Validation**: JWT via middleware de autenticación
- **Authorization**: Endpoints protegidos con `[Authorize]`
- **Claims**: Extracción de claims para autorización granular

```csharp
// JWT Middleware Setup
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = configuration["Supabase:Url"];
            options.Audience = "authenticated";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("CoachOnly", policy =>
            policy.RequireClaim("role", "coach"));
        options.AddPolicy("AthleteOrCoach", policy =>
            policy.RequireClaim("role", "athlete", "coach"));
    });
}
```

## Vertical Slice Architecture
Organizar endpoints por feature/caso de uso en lugar de por tipo:

```csharp
// Training Feature - Vertical Slice
namespace SportPlanner.API.Features.Training
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrainingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "CoachOnly")]
        public async Task<ActionResult<TrainingResponse>> CreateTraining(
            [FromBody] CreateTrainingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingResponse>> GetTraining(
            [FromRoute] GetTrainingQuery query)
        {
            var result = await _mediator.Send(query);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
```

## Domain Entities
```csharp
// Domain Entity Example
public class Training : Entity
{
    public string Name { get; private set; }
    public TimeSpan Duration { get; private set; }
    public TrainingIntensity Intensity { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public Guid CoachId { get; private set; }
    public Guid AthleteId { get; private set; }

    private Training() { } // EF Constructor

    public Training(string name, TimeSpan duration, TrainingIntensity intensity, 
                   DateTime scheduledDate, Guid coachId, Guid athleteId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Training name cannot be empty", nameof(name));
        
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive", nameof(duration));

        Name = name;
        Duration = duration;
        Intensity = intensity;
        ScheduledDate = scheduledDate;
        CoachId = coachId;
        AthleteId = athleteId;

        AddDomainEvent(new TrainingCreatedEvent(Id, name, coachId, athleteId));
    }

    public void UpdateSchedule(DateTime newDate)
    {
        if (newDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot schedule training in the past");

        ScheduledDate = newDate;
        AddDomainEvent(new TrainingRescheduledEvent(Id, newDate));
    }
}
```

## Application Layer - Use Cases
```csharp
// Command Handler Example
public class CreateTrainingCommandHandler : IRequestHandler<CreateTrainingCommand, TrainingResponse>
{
    private readonly SportPlannerDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateTrainingCommandHandler(SportPlannerDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TrainingResponse> Handle(CreateTrainingCommand request, CancellationToken cancellationToken)
    {
        // Validate coach permissions
        var coachId = _currentUser.GetUserId();
        if (!await _context.Coaches.AnyAsync(c => c.Id == coachId, cancellationToken))
            throw new UnauthorizedAccessException("Only coaches can create trainings");

        // Create training
        var training = new Training(
            request.Name,
            TimeSpan.FromMinutes(request.DurationMinutes),
            request.Intensity,
            request.ScheduledDate,
            coachId,
            request.AthleteId
        );

        _context.Trainings.Add(training);
        await _context.SaveChangesAsync(cancellationToken);

        return new TrainingResponse
        {
            Id = training.Id,
            Name = training.Name,
            Duration = training.Duration,
            Intensity = training.Intensity,
            ScheduledDate = training.ScheduledDate
        };
    }
}
```

## Error Handling Middleware
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException => new { error = "Validation failed", details = exception.Message },
            UnauthorizedAccessException => new { error = "Unauthorized access" },
            ArgumentException => new { error = "Invalid argument", details = exception.Message },
            _ => new { error = "Internal server error" }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => 400,
            UnauthorizedAccessException => 401,
            ArgumentException => 400,
            _ => 500
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Configuration and Dependency Injection
```csharp
// Program.cs (.NET 8)
var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<SportPlannerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Application Services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```
