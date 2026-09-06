using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Deposito
{
    public int IdDeposito { get; set; }

    public string? Nombre { get; set; }

    public string Direccion { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Ubicacione> Ubicaciones { get; set; } = new List<Ubicacione>();
}
