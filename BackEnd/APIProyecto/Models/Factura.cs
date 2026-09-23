using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Factura
{
    public long IdFactura { get; set; }

    public string TipoComprobante { get; set; } = null!;

    public string NroComprobante { get; set; } = null!;

    public DateOnly? FechaEmision { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public decimal Monto { get; set; }

    public decimal? Iva { get; set; }

    public string CondicionPago { get; set; } = null!;

    public string? Estado { get; set; }

    public string? ArchivoAdjunto { get; set; }

    public virtual ICollection<ComprasPendiente> ComprasPendientes { get; set; } = new List<ComprasPendiente>();

    public virtual ICollection<ComprasRealizada> ComprasRealizada { get; set; } = new List<ComprasRealizada>();

    public virtual ICollection<DetalleOrdenesPago> DetalleOrdenesPagos { get; set; } = new List<DetalleOrdenesPago>();

    public virtual ICollection<MovimientosCc> MovimientosCcs { get; set; } = new List<MovimientosCc>();

    public virtual ICollection<NotasCreditoDebito> NotasCreditoDebitos { get; set; } = new List<NotasCreditoDebito>();
}
