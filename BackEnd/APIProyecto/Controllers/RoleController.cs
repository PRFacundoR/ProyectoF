using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APIProyecto.Interfaces;
using APIProyecto.Models;
using APIProyecto.ViewModels;

namespace APIProyecto.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRepositorioRoles _repoRoles;
        private readonly IRepositorioPermisos _repoPermisos;

        public RolesController(IRepositorioRoles repoRoles, IRepositorioPermisos repoPermisos)
        {
            _repoRoles = repoRoles;
            _repoPermisos = repoPermisos;
        }


        [HttpGet("permisos")]
        public async Task<ActionResult<IEnumerable<PermisoViewModel>>> GetPermisos()
        {
            var permisosDb = await _repoPermisos.GetPermisos();

            var permisosViewModel = permisosDb.Select(p => new PermisoViewModel
            {
                IdPermiso = p.IdPermiso,
                NombrePermiso = p.NombrePermiso,
                Descripcion = p.Descripcion
            }).ToList();

            return Ok(permisosViewModel);
        }

        [HttpGet]
        [Authorize(Policy = "RequiereLeerRoles")]
        public async Task<ActionResult<IEnumerable<RoleViewModel>>> GetRoles()
        {
            var rolesDb = await _repoRoles.GetRoles();

            var rolesViewModel = rolesDb.Select(r => new RoleViewModel
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                PermisosAsignados = r.IdPermisos.Select(p => p.NombrePermiso).ToList()
            }).ToList();

            return Ok(rolesViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "RequiereCrearRoles")]
        public async Task<ActionResult> CrearRol([FromBody] RoleCreateViewModel modelo)
        {

            var permisosDb = await _repoPermisos.GetPermisos();
            var idsValidosDb = permisosDb.Select(p => p.IdPermiso).ToList();

            var permisosInexistentes = modelo.PermisosIds.Except(idsValidosDb).ToList();

            if (permisosInexistentes.Any())
            {
                return BadRequest(new
                {
                    mensaje = $"Error: Los siguientes IDs de permisos no existen en el sistema: {string.Join(", ", permisosInexistentes)}"
                });
            }

            bool existe = await _repoRoles.ExisteRol(modelo.Nombre);
            if (existe)
            {
                return BadRequest(new { mensaje = $"Ya existe un rol con el nombre '{modelo.Nombre}'." });
            }

            var nuevoRol = new Role
            {
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion
            };

            var rolCreado = await _repoRoles.CreateRol(nuevoRol, modelo.PermisosIds);

            return Ok(new { mensaje = "Rol creado exitosamente." });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarRol(int id, [FromBody] RoleCreateViewModel modelo)
        {

            if (id == 1)
            {
                return BadRequest(new { mensaje = "El rol de Administrador principal del sistema está protegido y no puede ser actualizado." });
            }

            var miRolActual = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var rolAactualizar = await _repoRoles.GetRol(id);
            
            if (rolAactualizar != null && rolAactualizar.Nombre == miRolActual)
            {
                return BadRequest(new { mensaje = "Seguridad: No puedes editar el rol que estás utilizando actualmente." });
            }
            
            var permisosDb = await _repoPermisos.GetPermisos();
            var idsValidosDb = permisosDb.Select(p => p.IdPermiso).ToList();

            var permisosInexistentes = modelo.PermisosIds.Except(idsValidosDb).ToList();

            if (permisosInexistentes.Any())
            {
                return BadRequest(new
                {
                    mensaje = $"Error: Los siguientes IDs de permisos no existen en el sistema: {string.Join(", ", permisosInexistentes)}"
                });
            }

            var rolesExistentes = await _repoRoles.GetRoles();
            if (rolesExistentes.Any(r => r.Nombre.ToLower() == modelo.Nombre.ToLower() && r.IdRol != id))
            {
                return BadRequest(new { mensaje = $"El nombre '{modelo.Nombre}' ya está siendo usado por otro rol." });
            }

            var rolActualizado = new Role
            {
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion
            };

            var resultado = await _repoRoles.UpdateRol(id, rolActualizado, modelo.PermisosIds);

            if (resultado == null)
            {
                return NotFound(new { mensaje = "El rol que intenta modificar no existe." });
            }

            return Ok(new { mensaje = "Rol actualizado exitosamente." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRol(int id)
        {

            if (id == 1)
            {
                return BadRequest(new { mensaje = "El rol de Administrador principal del sistema está protegido y no puede ser eliminado." });
            }

            // SEGURO DE VIDA 2: El usuario no puede borrar el rol que tiene puesto ahora mismo
            var miRolActual = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var rolABorrar = await _repoRoles.GetRol(id);

            if (rolABorrar != null && rolABorrar.Nombre == miRolActual)
            {
                return BadRequest(new { mensaje = "Seguridad: No puedes eliminar el rol que estás utilizando actualmente." });
            }
            var eliminado = await _repoRoles.DeleteRol(id);

            if (!eliminado)
            {
                return NotFound(new { mensaje = "El rol que intenta eliminar no existe." });
            }

            return Ok(new { mensaje = "Rol eliminado exitosamente." });
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<RoleViewModel>>> BuscarRoles([FromQuery] string nombre)
        {
            var rolesDb = await _repoRoles.GetRoles();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                // Filtramos ignorando mayúsculas y minúsculas
                rolesDb = rolesDb.Where(r => r.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var rolesViewModel = rolesDb.Select(r => new RoleViewModel
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                PermisosAsignados = r.IdPermisos.Select(p => p.NombrePermiso).ToList()
            }).ToList();

            return Ok(rolesViewModel);
        }


        // ========================================================================
        // GET: api/roles/5 (Trae UN rol específico para llenar el formulario de Editar)
        // ========================================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleViewModel>> GetRol(int id)
        {
            var rolDb = await _repoRoles.GetRol(id);

            if (rolDb == null)
            {
                return NotFound(new { mensaje = "El rol solicitado no existe." });
            }

            var rolViewModel = new RoleViewModel
            {
                IdRol = rolDb.IdRol,
                Nombre = rolDb.Nombre,
                Descripcion = rolDb.Descripcion,
                PermisosAsignados = rolDb.IdPermisos?.Select(p => p.NombrePermiso).ToList() ?? new List<string>()
            };

            return Ok(rolViewModel);
        }


    }

}