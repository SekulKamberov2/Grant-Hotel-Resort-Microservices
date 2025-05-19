using System.Data;
using System.Reflection;

using Microsoft.Data.SqlClient;

using MediatR;
using FluentValidation;

using GHR.DutyManagement.Application.Behaviors;
using GHR.DutyManagement.Application.Commands.AssignDuty;
using GHR.DutyManagement.Application.Interfaces;
using GHR.DutyManagement.Infrastructure.Repositories;
using GHR.DutyManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDutyRepository, DutyRepository>(); 

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
//builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AssignDutyCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
 
builder.Services.AddScoped<IDutyService, DutyService>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
 
app.Run();
