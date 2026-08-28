using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Ubicacione
{
    public int IdUbicacion { get; set; }

    public string Sector { get; set; } = null!;

    public string Estanteria { get; set; } = null!;

    public bool? Activo { get; set; }

    public int IdDeposito { get; set; }

    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    public virtual ICollection<StockUbicacion> StockUbicacions { get; set; } = new List<StockUbicacion>();
}
