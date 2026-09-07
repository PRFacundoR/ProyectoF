using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Proveedore
{
    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = null!;

    public string TipoSociedad { get; set; } = null!;

    public string Cuit { get; set; } = null!;

    public string Cbu { get; set; } = null!;

    public string? Alias { get; set; }

    public string Domicilio { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Email { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<MovimientosCc> MovimientosCcs { get; set; } = new List<MovimientosCc>();

    public virtual ICollection<OrdenesPago> OrdenesPagos { get; set; } = new List<OrdenesPago>();

    public virtual ICollection<ProductosProveedor> ProductosProveedors { get; set; } = new List<ProductosProveedor>();
}
