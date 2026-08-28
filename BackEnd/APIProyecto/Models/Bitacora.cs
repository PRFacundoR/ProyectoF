using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Bitacora
{
    public int IdLog { get; set; }

    public int? IdUsuario { get; set; }

    public string NombreEmpleado { get; set; } = null!;

    public DateTime? FechaHora { get; set; }

    public string Modulo { get; set; } = null!;

    public string Accion { get; set; } = null!;

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
