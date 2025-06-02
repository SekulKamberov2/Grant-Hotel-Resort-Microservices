using System.Data;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

using MediatR;

using FluentValidation;

using Leaverequests.Grpc;  

using GHR.EmployeeManagement.Application.Behaviors;
using GHR.EmployeeManagement.Application.Commands.Create;
using GHR.EmployeeManagement.Application.Commands.Delete;
using GHR.EmployeeManagement.Application.Commands.IncreaseSalary;
using GHR.EmployeeManagement.Application.Commands.Update;
using GHR.EmployeeManagement.Application.Queries.GetEmployeeById;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByDepartment;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByFacility;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByManager;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesByStatus;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesHiredAfter;
using GHR.EmployeeManagement.Application.Queries.GetEmployeesSalaryAbove;
using GHR.EmployeeManagement.Application.Queries.Search;
using GHR.EmployeeManagement.Application.Services;
using GHR.EmployeeManagement.Infrastructure.Repositories;
using GHR.SharedKernel;
 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<LeaverequestsService.LeaverequestsServiceClient>(options =>
{
    options.Address = new Uri("http://leave-management:5004");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler();
})
.ConfigureChannel(options =>
{
    options.HttpHandler = new HttpClientHandler();
    options.HttpVersion = new Version(2, 0);
});



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
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesHiredAfterQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesSalaryAboveQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesByManagerQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEmployeesByStatusQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<IncreaseSalaryCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeDTOValidator>();

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            RequireSignedTokens = false,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = key,

            SignatureValidator = (token, parameters) =>
            {
                var tokenParts = token.Split('.');
                if (tokenParts.Length != 3)
                    throw new SecurityTokenException("Invalid token format.");

                var payload = $"{tokenParts[0]}.{tokenParts[1]}";
                var incomingSignature = tokenParts[2];

                using var hmac = new HMACSHA256(key.Key);
                var computedSignature = JwtHelper.Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

                if (incomingSignature != computedSignature)
                    throw new SecurityTokenInvalidSignatureException("Token signature mismatch.");

                return new JsonWebToken(token);
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                Console.WriteLine($"KEY: {key}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
