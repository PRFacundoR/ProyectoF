using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class MetodosPagoOrden
{
    public int IdMetodo { get; set; }

    public string? TipoMetodo { get; set; }

    public decimal Monto { get; set; }

    public string? Referencia { get; set; }

    public int IdOrden { get; set; }

    public virtual OrdenesPago IdOrdenNavigation { get; set; } = null!;
}
