using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class ProductosProveedor
{
    public int IdProducto { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCosto { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;
}
