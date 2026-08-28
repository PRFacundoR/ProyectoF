using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class RecuperacionPassword
{
    public int IdRecuperacion { get; set; }

    public string CodigoToken { get; set; } = null!;

    public DateTime? FechaGeneracion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public bool? Usado { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
