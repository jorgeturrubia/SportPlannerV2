using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SportPlanner.Application;
using SportPlanner.Infrastructure;
using SportPlanner.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // For CurrentUserService
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers(options =>
    {
        // Require authentication by default on all endpoints
        // Endpoints that should be public must be marked with [AllowAnonymous]
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    })
    // Return JSON using camelCase property names to match frontend expectations
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Allow enums to be serialized/deserialized as strings instead of numbers
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Supabase JWT Authentication
// Supabase uses HS256 (HMAC-SHA256) for signing JWTs, not RSA
// Therefore we must use the JWT Secret directly, not JWKS endpoint
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var supabaseUrl = builder.Configuration["Supabase:Url"];
        var supabaseJwtSecret = builder.Configuration["Supabase:JwtSecret"];
        
        if (string.IsNullOrEmpty(supabaseJwtSecret))
        {
            throw new InvalidOperationException("Supabase:JwtSecret is not configured in appsettings.json");
        }
        
        // Create symmetric security key from JWT secret
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(supabaseJwtSecret)
        );
        
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // Supabase issuer format: https://{project-ref}.supabase.co/auth/v1
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            
            ValidateAudience = true,
            ValidAudience = "authenticated",
            
            ValidateLifetime = true,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            
            ClockSkew = TimeSpan.FromMinutes(1),
            
            // Map JWT claims to standard .NET claims
            NameClaimType = "sub",  // Map 'sub' to ClaimTypes.NameIdentifier
            RoleClaimType = "role"  // Map 'role' to ClaimTypes.Role (if needed)
        };
        
        // Add event handler for debugging token validation
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT] Token validated successfully");
                Console.WriteLine($"[JWT] User: {context.Principal?.Identity?.Name}");
                Console.WriteLine($"[JWT] Claims count: {context.Principal?.Claims.Count()}");
                return Task.CompletedTask;
            }
        };
    });

// CORS - Configure different policies for Development and Production
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development: Allow local Angular dev server
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for cookies
        });
    }
    else
    {
        // Production: Only allow configured origins
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only redirect to HTTPS in production (not in development)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Security Headers - Protection against common web vulnerabilities
app.Use(async (context, next) =>
{
    // Prevent MIME type sniffing
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Prevent clickjacking attacks
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Enable XSS protection (legacy browsers)
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Control referrer information
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // HSTS - Force HTTPS (only in production)
    if (!context.Request.Host.Host.Contains("localhost"))
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await next();
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
