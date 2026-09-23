using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Catalogo
{
    public string CBarrasProveedor { get; set; } = null!;

    public int IdProveedor { get; set; }

    public int IdProducto { get; set; }

    public decimal PrecioCosto { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual ICollection<DetalleCompraP> DetalleCompraPs { get; set; } = new List<DetalleCompraP>();

    public virtual ICollection<DetalleCompraR> DetalleCompraRs { get; set; } = new List<DetalleCompraR>();

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Proveedore IdProveedorNavigation { get; set; } = null!;

    public virtual ICollection<ItemsPedido> ItemsPedidos { get; set; } = new List<ItemsPedido>();
}
