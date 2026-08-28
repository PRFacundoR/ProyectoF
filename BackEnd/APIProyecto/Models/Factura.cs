using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public string? TipoComprobante { get; set; }

    public string NroComprobante { get; set; } = null!;

    public DateOnly? FechaEmision { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal Monto { get; set; }

    public decimal? Iva { get; set; }

    public string? Estado { get; set; }

    public string? ArchivoAdjunto { get; set; }

    public int IdCompra { get; set; }

    public virtual Compra IdCompraNavigation { get; set; } = null!;
}
