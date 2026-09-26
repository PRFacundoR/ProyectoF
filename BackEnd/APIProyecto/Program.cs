using APIProyecto.DB;
using Microsoft.EntityFrameworkCore;
using APIProyecto.Interfaces;
using APIProyecto.Repository;
using Serilog;

//guardar error en archivo
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Para seguir viéndolos en la consola
    .WriteTo.File("Logs/errores_api-.txt", rollingInterval: RollingInterval.Day) // Crea un archivo nuevo por día
    .CreateLogger();




var builder = WebApplication.CreateBuilder(args);

//para guardar errores en archivo
builder.Host.UseSerilog(); 

// Controllers
builder.Services.AddControllers();

// HTTP Context
builder.Services.AddHttpContextAccessor();

// Base de datos
var connectionString =
    DatabaseConfig.BuildConnectionString(builder.Configuration);

builder.Services.AddDbContext<ComprasDbContext>(options =>
    options.UseNpgsql(connectionString));

// OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






























    builder.Services.AddScoped<IRepositorioRoles, RepositorioRoles>();
    builder.Services.AddScoped<IRepositorioPermisos, RepositorioPermisos>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", app =>
    {
        app.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("PermitirReact");

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();