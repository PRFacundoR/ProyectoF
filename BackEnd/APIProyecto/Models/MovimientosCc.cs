using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class MovimientosCc
{
    public int IdMovimiento { get; set; }

    public DateTime? FechaHora { get; set; }

    public string? TipoMovimiento { get; set; }

    public decimal Monto { get; set; }

    public int? IdCompra { get; set; }

    public int? IdNota { get; set; }

    public int? IdOrden { get; set; }

    public int IdProveedor { get; set; }

    public virtual Compra? IdCompraNavigation { get; set; }

    public virtual NotasCreditoDebito? IdNotaNavigation { get; set; }

    public virtual OrdenesPago? IdOrdenNavigation { get; set; }

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
