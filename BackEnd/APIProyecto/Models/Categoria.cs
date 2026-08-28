using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Producto> IdProductos { get; set; } = new List<Producto>();
}
