using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Weather.core.Entities;
using Weather.infra.Data;
using Weather.infra.ExternalClients;
using Weather.infra.Repositories;
using Weather.infra.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sql => sql.MigrationsAssembly("Weather.infra")
    ));

builder.Services.AddScoped<WeatherCompleteRepository>();
builder.Services.AddScoped<WeatherCompleteService>();

builder.Services.AddDataProtection();

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient<WeatherClient>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//app.UseSwagger();
//app.UseSwaggerUI();

//app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
