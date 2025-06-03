using GHR.DutyManagement.Services;
using GHR.RoomManagement.Repositories;
using GHR.RoomManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IDutyService, DutyService>(client =>
{
    client.BaseAddress = new Uri("http://duty-service:8080");
});
 

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllers();
 
builder.Services.AddOpenApi();

var app = builder.Build();

 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
