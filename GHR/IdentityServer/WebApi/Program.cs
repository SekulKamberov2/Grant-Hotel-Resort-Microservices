using System.Data;
using System.Reflection;

using Microsoft.Data.SqlClient; 

using MediatR;
using FluentValidation;

using IdentityServer.Application.Interfaces;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Application.Commands.CreateUser;
using IdentityServer.Application.Behaviors;
using IdentityServer.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using IdentityServer.Domain.Exceptions; 

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<IRoleRepository, RoleRepository>(); 

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRoleManager, RoleManager>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddControllers();

var app = builder.Build(); 

app.MapControllers();

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
            RepositoryException => StatusCodes.Status500InternalServerError, // Add specific cases
            _ => StatusCodes.Status500InternalServerError
        };

        // Log the exception
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>(); // Get logger
        logger.LogError(exception, "An error occurred.");

        // Write the response
        context.Response.StatusCode = statusCode;
        var errorResponse = new { error = exception?.Message, type = exception?.GetType().Name, timestamp = DateTime.UtcNow };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    });
});



app.Run();
 