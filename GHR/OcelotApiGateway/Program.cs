using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGateway;
using System.Net.Http.Headers;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
 
builder.Services.AddHttpContextAccessor();
 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3003")   
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://identity-service:8081";  
        options.RequireHttpsMetadata = false;
        options.Audience = "api";  
         
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });
 
builder.Services.AddOcelot()
    .AddSingletonDefinedAggregator<CustomAggregator>()
    .AddDelegatingHandler<AddAuthorizationHeaderHandler>(true);  

var app = builder.Build();
 
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
 
public class AddAuthorizationHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddAuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Cookies.TryGetValue("access_token", out var token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
