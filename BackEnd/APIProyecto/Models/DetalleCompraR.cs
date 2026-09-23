using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class DetalleCompraR
{
    public int IdCompra { get; set; }

    public string CBarrasProveedor { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual Catalogo CBarrasProveedorNavigation { get; set; } = null!;

    public virtual ComprasRealizada IdCompraNavigation { get; set; } = null!;
}
