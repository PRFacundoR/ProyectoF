using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class ComprasRealizada
{
    public int IdUsuario { get; set; }

    public int IdCompra { get; set; }

    public long IdFactura { get; set; }

    public int IdPedido { get; set; }

    public DateOnly FechaPedido { get; set; }

    public DateOnly FechaCompra { get; set; }

    public DateOnly? FechaEntrega { get; set; }

    public virtual ICollection<DetalleCompraR> DetalleCompraRs { get; set; } = new List<DetalleCompraR>();

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
