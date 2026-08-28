using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class DetalleOrdenesPago
{
    public int IdOrden { get; set; }

    public int IdCompra { get; set; }

    public decimal MontoAsignado { get; set; }

    public virtual Compra IdCompraNavigation { get; set; } = null!;

    public virtual OrdenesPago IdOrdenNavigation { get; set; } = null!;
}
