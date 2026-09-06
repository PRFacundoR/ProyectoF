using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Tarea
{
    public int IdTarea { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaLimite { get; set; }

    public string Estado { get; set; } = null!;

    public int IdEmpleado { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
