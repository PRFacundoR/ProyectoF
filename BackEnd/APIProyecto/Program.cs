using APIProyecto.DB;
using Microsoft.EntityFrameworkCore;
using APIProyecto.Interfaces;
using APIProyecto.Repository;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; 

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
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorización JWT. Escriba 'Bearer' [espacio] y luego su token.\r\n\r\nEjemplo: 'Bearer eyJhbGci...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});





























    builder.Services.AddScoped<IRepositorioRoles, RepositorioRoles>();
    builder.Services.AddScoped<IRepositorioPermisos, RepositorioPermisos>();













builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Tu puerto de React
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Necesario para SignalR
    });
});


// ==========================================================
// 2. CONFIGURACIÓN JWT
// ==========================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// ==========================================================
// 3. POLÍTICAS DE AUTORIZACIÓN (Basadas en la DB)
// ==========================================================
builder.Services.AddAuthorization(options =>
{
    // Protegemos con los permisos atómicos que creamos en SQL
    options.AddPolicy("RequiereLeerRoles", policy => policy.RequireClaim("Permiso", "LEER_ROLES"));
    options.AddPolicy("RequiereCrearRoles", policy => policy.RequireClaim("Permiso", "CREAR_ROLES"));
    options.AddPolicy("RequiereLeerUsuarios", policy => policy.RequireClaim("Permiso", "LEER_USUARIOS"));
    // A medida que necesites proteger controllers, agregás la política acá...
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("PermitirReact");
app.UseAuthentication(); // ¡Primero te identifico!
app.UseAuthorization();  // ¡Después veo qué podés hacer!
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