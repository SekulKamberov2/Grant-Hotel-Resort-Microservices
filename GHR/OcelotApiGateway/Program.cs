using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot services
builder.Services.AddOcelot();

var app = builder.Build();

// Use Ocelot middleware
app.UseOcelot().Wait();

app.Run();
