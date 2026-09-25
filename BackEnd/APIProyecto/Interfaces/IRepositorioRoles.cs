using APIProyecto.Models;   

namespace APIProyecto.Interfaces
{
    
    public interface IRepositorioRoles
    {
        Task<List<Role>> GetRoles();
        Task<Role> GetRol(int id);
        Task<Role> CreateRol(Role rol, List<int> permisoIds);
        Task<Role> UpdateRol(int id, Role rol, List<int> permisoIds);
        Task<bool> DeleteRol(int id);

         Task<bool> ExisteRol(string nombre);


    }
}