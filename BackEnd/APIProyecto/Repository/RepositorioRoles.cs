using APIProyecto.DB;
using APIProyecto.Models;
using APIProyecto.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APIProyecto.Repository
{

    public class RepositorioRoles : IRepositorioRoles
    {
        private readonly ComprasDbContext _contexto;
        private readonly ILogger<RepositorioRoles> _logger;

        public RepositorioRoles(ComprasDbContext contexto, ILogger<RepositorioRoles> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }


        public async Task<bool> ExisteRol(string nombre)
        {
            return await _contexto.Roles.AnyAsync(r => r.Nombre.ToLower() == nombre.ToLower());
        }
        public async Task<List<Role>> GetRoles()
        {
            try
            {
                return await _contexto.Roles.Include(r => r.IdPermisos).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles");
                throw;

            }

        }

        public async Task<Role> GetRol(int id)
        {
            try
            {
                return await _contexto.Roles
                    .Include(r => r.IdPermisos)
                    .FirstOrDefaultAsync(r => r.IdRol == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el rol con ID {id}.");
                throw;
            }
        }

        public async Task<Role> CreateRol(Role rol, List<int> permisoIds)
        {
            try
            {
                // Si mandaron permisos, los buscamos y se los enganchamos al rol antes de guardarlo
                if (permisoIds != null && permisoIds.Any())
                {
                    var permisos = await _contexto.Permisos
                        .Where(p => permisoIds.Contains(p.IdPermiso))
                        .ToListAsync();

                    foreach (var permiso in permisos)
                    {
                        rol.IdPermisos.Add(permiso);
                    }
                }

                _contexto.Roles.Add(rol);
                await _contexto.SaveChangesAsync();

                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol.");
                throw;
            }
        }



        public async Task<Role> UpdateRol(int id, Role rol, List<int> permisoIds)
        {
            try
            {
                // CLAVE: Tenemos que incluir los permisos actuales (.Include) para poder modificarlos
                var rolExistente = await _contexto.Roles
                    .Include(r => r.IdPermisos)
                    .FirstOrDefaultAsync(r => r.IdRol == id);

                // Si no existe, devolvemos null para que el Controller tire un 404 Not Found
                if (rolExistente == null) return null;

                // 1. Actualizamos los campos de texto
                rolExistente.Nombre = rol.Nombre;
                rolExistente.Descripcion = rol.Descripcion;

                // 2. Limpiamos los permisos viejos que tenía
                rolExistente.IdPermisos.Clear();

                // 3. Si nos mandaron permisos nuevos, los buscamos y los asignamos
                if (permisoIds != null && permisoIds.Any())
                {
                    var nuevosPermisos = await _contexto.Permisos
                        .Where(p => permisoIds.Contains(p.IdPermiso))
                        .ToListAsync();

                    foreach (var permiso in nuevosPermisos)
                    {
                        rolExistente.IdPermisos.Add(permiso);
                    }
                }

                // 4. Guardamos todos los cambios (Texto y Permisos) en un solo viaje a la BD
                await _contexto.SaveChangesAsync();

                return rolExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el rol con ID {id}.");
                throw;
            }
        }

        public async Task<bool> DeleteRol(int id)
        {
            try
            {
                var rolExistente = await _contexto.Roles
            .Include(r => r.IdPermisos) // o Include(r => r.RolesPermisos) si tu modelo usa la tabla intermedia explícita
            .FirstOrDefaultAsync(r => r.IdRol == id);

        if (rolExistente == null) return false;

        // 1. Limpiamos las relaciones (Esto borra las filas en roles_permisos)
        rolExistente.IdPermisos.Clear();

        // 2. Ahora sí borramos el rol
        _contexto.Roles.Remove(rolExistente);
        await _contexto.SaveChangesAsync();
        
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error al eliminar el rol con ID {id}.");
        throw;
    }
}




    }


}