using FluentValidation;
using GHR.EmployeeManagement.Application.Behaviors;
using GHR.EmployeeManagement.Application.Commands.Create;
using GHR.EmployeeManagement.Application.Commands.Delete;
using GHR.EmployeeManagement.Application.Commands.Update;
using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility;
using GHR.EmployeeManagement.Application.Queries.Search;
using GHR.EmployeeManagement.Application.Services;
using GHR.EmployeeManagement.Infrastructure.Repositories;
using MediatR;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); 
builder.Services.AddScoped<IEmployeeService, EmployeeService>(); 
builder.Services.AddScoped<IOnBoardingService, OnBoardingService>(); 

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdQueryValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeeByIdQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEmployeeCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteEmployeeCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SearchEmployeesByNameQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesByDepartmentQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesByFacilityQueryValidator>();

builder.Services.AddControllers(); 

var app = builder.Build();
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
