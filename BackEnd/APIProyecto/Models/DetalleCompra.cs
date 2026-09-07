using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class DetalleCompra
{
    public int IdProducto { get; set; }

    public int IdCompra { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int IdProveedor { get; set; }

    public virtual Compra IdCompraNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
