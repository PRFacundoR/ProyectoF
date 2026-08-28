using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Turno
{
    public int IdTurno { get; set; }

    public string Turno1 { get; set; } = null!;

    public virtual ICollection<TurnosDuracion> TurnosDuracions { get; set; } = new List<TurnosDuracion>();
}
