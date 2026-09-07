using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Compra
{
    public int IdUsuario { get; set; }

    public int IdCompra { get; set; }

    public DateOnly FechaCompra { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<DetalleOrdenesPago> DetalleOrdenesPagos { get; set; } = new List<DetalleOrdenesPago>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosCc> MovimientosCcs { get; set; } = new List<MovimientosCc>();
}
