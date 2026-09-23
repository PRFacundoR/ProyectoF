using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class ComprasPendiente
{
    public int IdUsuario { get; set; }

    public int IdCompra { get; set; }

    public long? IdFactura { get; set; }

    public DateOnly FechaPedido { get; set; }

    public DateOnly FechaCompra { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public bool Incompleta { get; set; }

    public virtual ICollection<DetalleCompraP> DetalleCompraPs { get; set; } = new List<DetalleCompraP>();

    public virtual Factura? IdFacturaNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
