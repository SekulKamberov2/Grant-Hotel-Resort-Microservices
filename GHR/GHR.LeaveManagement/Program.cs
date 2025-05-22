using GHR.LeaveManagement.Mapping;
using GHR.LeaveManagement.Repositories;
using GHR.LeaveManagement.Repositories.Interfaces;
using System.Data;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
 
using GHR.LeaveManagement.Services;
using GHR.LeaveManagement.Services.Interfaces;
using GHR.SharedKernel;
 
var builder = WebApplication.CreateBuilder(args);

 
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));


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
builder.Services.AddControllers();
 
builder.Services.AddOpenApi();

var app = builder.Build(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
