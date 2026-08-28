using APIProyecto.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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