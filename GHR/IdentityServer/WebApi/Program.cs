using System.Data;
using System.Reflection;
using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;

using FluentValidation;
using MediatR;

using AutoMapper;
using IdentityServer.Application.Common.Mappings;

using IdentityServer.Application.Behaviors;
using IdentityServer.Application.Commands.CreateUser; 
using IdentityServer.Application.Exceptions;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Exceptions;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Infrastructure.Repositories;
 
var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support HTTP/2 (for gRPC) on port 5000 without HTTPS 
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; //gRPC
    });
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1; // REST API
    });
});

// Add services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGHRclient", policy =>
    {
        policy.WithOrigins("http://localhost:3003/")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRoleManager, RoleManager>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddControllers();
builder.Services.AddGrpc();

var app = builder.Build();

// Map gRPC services
app.MapGrpcService<IdentityGrpcService>();

// Map REST controllers
app.MapControllers();

// Use CORS
app.UseCors("AllowGHRclient");

// Global exception handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            RepositoryException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An error occurred.");

        var errorResponse = new
        {
            error = exception?.Message,
            type = exception?.GetType().Name,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    });
});

app.Run();
