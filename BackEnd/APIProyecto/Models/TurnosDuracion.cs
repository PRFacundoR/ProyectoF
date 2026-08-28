using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class TurnosDuracion
{
    public int IdTurno { get; set; }

    public int IdEmpleado { get; set; }

    public DateOnly? Inicio { get; set; }

    public DateOnly? Fin { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Turno IdTurnoNavigation { get; set; } = null!;
}
