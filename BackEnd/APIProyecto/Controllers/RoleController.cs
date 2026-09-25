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
        public async Task<ActionResult> CrearRol([FromBody] RoleCreateViewModel modelo)
        {
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
            var eliminado = await _repoRoles.DeleteRol(id);
            
            if (!eliminado)
            {
                return NotFound(new { mensaje = "El rol que intenta eliminar no existe." });
            }

            return Ok(new { mensaje = "Rol eliminado exitosamente." });
        }

       



    }
    
}