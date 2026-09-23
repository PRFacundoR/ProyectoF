using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class StockUbicacion
{
    public int IdProducto { get; set; }

    public int IdUbicacion { get; set; }

    public int? Cantidad { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Ubicacione IdUbicacionNavigation { get; set; } = null!;
}
