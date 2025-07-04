using Core.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infraestructura.Data;
using Infraestructura.Repositories;
using Infraestructura.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Weather.api.Validators;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


// Data & Domain Services
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(
        config.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("Weather.infra")
    )
);

builder.Services.AddScoped<WeatherCompleteRepository>();
builder.Services.AddScoped<WeatherCompleteService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();


// External Client
builder.Services.AddSingleton(_ =>
{
    var options = new RestClientOptions("https://www.el-tiempo.net/api/json/v2/")
    {
        // configure timeout, proxy, etc.
    };
    return new RestClient(options);
});
builder.Services.AddTransient<WeatherClient>();


// Identity & Data Protection
builder.Services.AddDataProtection();
builder.Services
    .AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// JWT Configuration
var jwtSection = config.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

// Limpia el mapeo automático de claims para que "nameid" y "securityStamp" lleguen con ese mismo nombre
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        // 1) Parámetros básicos de validación
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // 2) Aquí enganchamos el evento que comprueba el SecurityStamp
        opts.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                // obtenemos el UserManager desde DI
                var userMgr = ctx.HttpContext.RequestServices
                                     .GetRequiredService<UserManager<Usuario>>();

                // extraemos el userId y el stamp que vino en el token
                var userId = ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
                var stampInToken = ctx.Principal?.FindFirstValue("securityStamp");

                // buscamos el usuario en BD y comparamos
                var user = await userMgr.FindByIdAsync(userId!);
                if (user == null || user.SecurityStamp != stampInToken)
                {
                    ctx.Fail("Token inválido: SecurityStamp no coincide.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isAdmin", policy =>
        policy.RequireClaim("isAdmin", "true"));

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CredentialsUserDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditClaimDTOValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather API",
        Version = "v1",
        Description = "API The time"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce:{token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();


// Middleware

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API 1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();

