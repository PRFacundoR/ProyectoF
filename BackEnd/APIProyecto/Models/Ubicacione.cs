using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Ubicacione
{
    public int IdUbicacion { get; set; }

    public string Sector { get; set; } = null!;

    public short Estanteria { get; set; }

    public bool Activo { get; set; }

    public int IdDeposito { get; set; }

    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
