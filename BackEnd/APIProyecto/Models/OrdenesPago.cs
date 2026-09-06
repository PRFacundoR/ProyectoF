using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class OrdenesPago
{
    public int IdOrden { get; set; }

    public DateOnly FechaEmision { get; set; }

    public decimal MontoTotal { get; set; }

    public string? ArchivoPdf { get; set; }

    public int IdAutoriza { get; set; }

    public int IdEmisor { get; set; }

    public int IdProveedor { get; set; }

    public virtual ICollection<DetalleOrdenesPago> DetalleOrdenesPagos { get; set; } = new List<DetalleOrdenesPago>();

    public virtual Usuario IdAutorizaNavigation { get; set; } = null!;

    public virtual Usuario IdEmisorNavigation { get; set; } = null!;

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;

    public virtual ICollection<MetodosPagoOrden> MetodosPagoOrdens { get; set; } = new List<MetodosPagoOrden>();

    public virtual ICollection<MovimientosCc> MovimientosCcs { get; set; } = new List<MovimientosCc>();
}
