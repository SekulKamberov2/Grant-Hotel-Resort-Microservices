using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGateway;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
// Add JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://identity-service:8081"; //  your identity service URL
        options.RequireHttpsMetadata = false;
        options.Audience = "api"; // match your token audience if applicable
    });

// Add Ocelot and register aggregator
builder.Services.AddOcelot()
    .AddSingletonDefinedAggregator<CustomAggregator>();

var app = builder.Build();

app.UseAuthentication(); // Add this
app.UseAuthorization();

await app.UseOcelot();
app.Run();
