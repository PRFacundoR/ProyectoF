using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? CodigoBarras { get; set; }

    public int? StockMinimo { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<ProductosProveedor> ProductosProveedors { get; set; } = new List<ProductosProveedor>();

    public virtual ICollection<StockUbicacion> StockUbicacions { get; set; } = new List<StockUbicacion>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
