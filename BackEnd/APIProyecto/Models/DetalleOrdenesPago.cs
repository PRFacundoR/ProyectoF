using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class DetalleOrdenesPago
{
    public long IdOrden { get; set; }

    public long IdFactura { get; set; }

    public decimal MontoAsignado { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual OrdenesPago IdOrdenNavigation { get; set; } = null!;
}
