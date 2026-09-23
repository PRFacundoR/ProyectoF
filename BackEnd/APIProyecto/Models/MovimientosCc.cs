using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class MovimientosCc
{
    public long IdMovimiento { get; set; }

    public DateTime? FechaHora { get; set; }

    public string? TipoMovimiento { get; set; }

    public decimal Monto { get; set; }

    public long? IdFactura { get; set; }

    public long? IdNota { get; set; }

    public long? IdOrden { get; set; }

    public int IdProveedor { get; set; }

    public virtual Factura? IdFacturaNavigation { get; set; }

    public virtual NotasCreditoDebito? IdNotaNavigation { get; set; }

    public virtual OrdenesPago? IdOrdenNavigation { get; set; }

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
