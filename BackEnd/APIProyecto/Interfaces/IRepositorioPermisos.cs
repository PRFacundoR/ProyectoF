using APIProyecto.Models;   

namespace APIProyecto.Interfaces
{
    
    public interface IRepositorioPermisos
    {
        Task<List<Permiso>> GetPermisos();
        Task<Permiso> GetPermiso(int id);
        
        

    }

}