using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Comidify.API.Data;
using Comidify.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar puertos
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
}
else
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Determinar ruta de la base de datos
var dbPath = Path.Combine(
    Environment.GetEnvironmentVariable("HOME") ?? Environment.CurrentDirectory,
    "data",
    "comidify.db"
);

Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
Console.WriteLine($"Base de datos SQLite en: {dbPath}");

// Add services
builder.Services.AddControllers();
builder.Services.AddDbContext<ComidifyDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Registrar servicios
builder.Services.AddScoped<ITokenService, TokenService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "https://nice-dune-04db8720f.2.azurestaticapps.net" // Cambia esto por tu URL de Static Web App
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");

// DEBUG - Imprimir configuración JWT
Console.WriteLine("==============================================");
Console.WriteLine("JWT CONFIGURATION:");
Console.WriteLine($"SigningKey: {jwtSettings["SigningKey"]}");
Console.WriteLine($"Issuer: {jwtSettings["Issuer"]}");
Console.WriteLine($"Audience: {jwtSettings["Audience"]}");
Console.WriteLine("==============================================");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]!)),
            ValidateIssuer = true,            // ← TRUE de nuevo
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,          // ← TRUE de nuevo
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,          // ← TRUE de nuevo
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine($"=== OnMessageReceived ===");
                Console.WriteLine($"Token: {context.Token}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"=== Token Validated Successfully ===");
                var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"User ID from token: {userId}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"=== Authentication Failed ===");
                Console.WriteLine($"Exception: {context.Exception.GetType().Name}");
                Console.WriteLine($"Message: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"=== OnChallenge ===");
                Console.WriteLine($"Error: {context.Error}");
                Console.WriteLine($"ErrorDescription: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Comidify API", 
        Version = "v1" 
    });

    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,      // ← Asegúrate que sea Http
        Scheme = "bearer",                    // ← lowercase
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega SOLO el token (sin 'Bearer')."
    });
    // Carlos
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ComidifyDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("Base de datos inicializada correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar base de datos: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Backend listo. Swagger disponible en /swagger");

app.Run();