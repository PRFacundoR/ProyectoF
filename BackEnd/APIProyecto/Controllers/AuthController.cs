using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIProyecto.DB;
using Microsoft.EntityFrameworkCore;

namespace APIProyecto.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ComprasDbContext _contexto;
        private readonly IConfiguration _config;

        public AuthController(ComprasDbContext contexto, IConfiguration config)
        {
            _contexto = contexto;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel modelo)
        {
            // 1. Buscamos el usuario y TRAEMOS EL ROL CON SUS PERMISOS
            var usuarioDb = await _contexto.Usuarios
                .Include(u => u.IdUsuarioNavigation) // Traemos los datos del Empleado (Nombre)
                .Include(u => u.IdRolNavigation)     // Traemos el Rol
                    .ThenInclude(r => r.IdPermisos)  // Traemos los Permisos de ese Rol
                .FirstOrDefaultAsync(u => u.Email == modelo.Email && u.PasswordHash == modelo.Password);

            // Validar credenciales y si el empleado está activo
            if (usuarioDb == null || !usuarioDb.IdUsuarioNavigation.Activo)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
            }

            // 2. Armamos los "Claims" (Identificaciones) del Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDb.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuarioDb.IdUsuarioNavigation.Nombre),
                new Claim(ClaimTypes.Email, usuarioDb.Email),
                new Claim(ClaimTypes.Role, usuarioDb.IdRolNavigation.Nombre)
            };

            // 3. EL SECRETO DE LA SEGURIDAD: Inyectar cada permiso atómico en el token
            foreach (var permiso in usuarioDb.IdRolNavigation.IdPermisos)
            {
                claims.Add(new Claim("Permiso", permiso.NombrePermiso)); // Ej: "LEER_ROLES"
            }

            // 4. Firmar el Token
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // El token dura 8 horas
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Devolvemos el Token al Front-end
            return Ok(new { 
                token = tokenHandler.WriteToken(token),
                usuario = usuarioDb.IdUsuarioNavigation.Nombre,
                rol = usuarioDb.IdRolNavigation.Nombre,
                permisos = usuarioDb.IdRolNavigation.IdPermisos.Select(p => p.NombrePermiso).ToList()
            });
        }
    }
}