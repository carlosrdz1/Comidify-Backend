using Microsoft.EntityFrameworkCore;
using Comidify.API.Data;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();


// Configurar puertos
if (builder.Environment.IsDevelopment())
{
    // En desarrollo local: usar puertos estándar
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
}
else
{
    // En producción (Azure): usar variable de entorno PORT
    
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Determinar ruta de la base de datos
var dbPath = Path.Combine(
    Environment.GetEnvironmentVariable("HOME") ?? Environment.CurrentDirectory,
    "data",
    "comidify.db"
);

// Crear directorio si no existe
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

Console.WriteLine($"Base de datos SQLite en: {dbPath}");

// Add services to the container
builder.Services.AddControllers();

// Configurar DbContext con SQLite
builder.Services.AddDbContext<ComidifyDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migraciones automáticamente
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

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();



// Swagger siempre disponible
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();


Console.WriteLine($"Backend corriendo en puerto {port}");

app.Run();

app.MapGet("/ping", () => "pong");

app.Run();
