using APIProyecto.DB;
using APIProyecto.Models;
using APIProyecto.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APIProyecto.Repository
{
    public class RepositorioPermisos : IRepositorioPermisos
    {
        private readonly ComprasDbContext _contexto;
        private readonly ILogger<RepositorioPermisos> _logger;

        public RepositorioPermisos(ComprasDbContext contexto, ILogger<RepositorioPermisos> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public async Task<List<Permiso>> GetPermisos()
        {
            try
            {
                return await _contexto.Permisos.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de permisos.");
                throw;
            }
        }

        public async Task<Permiso> GetPermiso(int id)
        {
            try
            {
                return await _contexto.Permisos.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el permiso con ID {id}.");
                throw;
            }
        }
    }
}