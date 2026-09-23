using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? CodigoBarras { get; set; }

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Catalogo> Catalogos { get; set; } = new List<Catalogo>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
