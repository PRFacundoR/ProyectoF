using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string? FirmaDig { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual ICollection<TurnosDuracion> TurnosDuracions { get; set; } = new List<TurnosDuracion>();

    public virtual Usuario? Usuario { get; set; }
}
