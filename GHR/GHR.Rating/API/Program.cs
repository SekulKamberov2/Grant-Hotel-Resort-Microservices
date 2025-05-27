using System.Data;
using System.Security.Cryptography;
using System.Text;
 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using MediatR;
using FluentValidation;
 
using GHR.Rating.Application.Commands;
using GHR.Rating.Application.Commands.ApproveRating;
using GHR.Rating.Application.Commands.AwardTopPerformers;
using GHR.Rating.Application.Commands.BulkDeleteRatings;
using GHR.Rating.Application.Commands.CreateAward;
using GHR.Rating.Application.Commands.CreateRating;
using GHR.Rating.Application.Commands.DeleteAward;
using GHR.Rating.Application.Commands.DeleteRating;
using GHR.Rating.Application.Commands.RestoreRating;
using GHR.Rating.Application.Commands.UpdateAward;
using GHR.Rating.Application.Commands.UpdateRating;
using GHR.Rating.Application.Queries;
using GHR.Rating.Application.Queries.GetAllRatings;
using GHR.Rating.Application.Queries.GetAverageRatin;
using GHR.Rating.Application.Queries.GetAwardById;
using GHR.Rating.Application.Queries.GetAwardsByPeriod;
using GHR.Rating.Application.Queries.GetRankingByPeriod;
using GHR.Rating.Application.Queries.GetRatingById;
using GHR.Rating.Application.Queries.GetRatingsByStatus;
using GHR.Rating.Application.Queries.Validators;
using GHR.Rating.Application.Services;
using GHR.Rating.Application.Validators;
using GHR.Rating.Domain.Repositories;
using GHR.Rating.Infrastructure.Repositories;
using GHR.SharedKernel;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IAwardService, AwardService>();
builder.Services.AddScoped<IAwardRepository, AwardRepository>();

builder.Services.AddMediatR(typeof(CreateRatingCommandHandler).Assembly);
//builder.Services.AddFluentValidationAutoValidation(); // for automatic model validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateRatingCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RestoreRatingCommandValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<ApproveRatingCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BulkDeleteRatingsCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteRatingCommandHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<FlagRatingCommandValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<UnflagRatingCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRatingCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetAllRatingsQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetAverageRatingQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRankingByPeriodQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRatingByIdQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRatingsByDepartmentQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRatingsByServiceQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRatingsByStatusQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetRatingsByUserQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AwardTopPerformersCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAwardCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteAwardCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetAwardByIdQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetAwardsByPeriodQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateAwardCommandValidator>(); 

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
