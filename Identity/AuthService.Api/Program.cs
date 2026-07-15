using AuthService.Application.Interfaces;
using AuthService.Application.UseCases;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<RegisterUserCommand>();
builder.Services.AddScoped<LoginUserCommand>();
builder.Services.AddScoped<GetUserDetailsQuery>();

// Client registry (one entry per portfolio project consuming this service)
var clients = configuration.GetSection("Clients").Get<Dictionary<string, ClientConfig>>()
              ?? new Dictionary<string, ClientConfig>();
var allowedOrigins = clients.Values
    .SelectMany(c => c.AllowedOrigins)
    .Distinct()
    .ToArray();

builder.Services.AddSingleton<IClientRegistry>(new ClientRegistry(clients.Keys));

// JWT
var jwtSecret = configuration["Jwt:Secret"]!;
builder.Services.AddSingleton<ITokenService>(new JwtTokenService(jwtSecret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// CORS: allow the origins declared by every registered client project
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// Maps known application exceptions to HTTP status codes instead of an unhandled 500
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (UnauthorizedAccessException ex)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.UseSwagger();
app.UseSwaggerUI();


// ENABLE CORS BEFORE AUTH
app.UseCors("AllowClients");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

internal class ClientConfig
{
    public List<string> AllowedOrigins { get; set; } = new();
}