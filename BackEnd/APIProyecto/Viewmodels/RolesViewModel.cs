using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIProyecto.ViewModels
{
    public class RoleViewModel
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public List<string> PermisosAsignados { get; set; } = new List<string>();
    }
}



namespace APIProyecto.ViewModels
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción del rol es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Required(ErrorMessage = "Debe asignar al menos un permiso al rol.")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos 1 permiso.")]
        public List<int> PermisosIds { get; set; } = new List<int>();
    }
}


public class RoleUpdateViewModel
{
    [Required]
    public int IdRol { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Debe asignar al menos un permiso al rol.")]
    public List<int> PermisosIds { get; set; } = new List<int>();
}

