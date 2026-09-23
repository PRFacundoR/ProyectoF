using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Pedido
{
    public int IdUsuario { get; set; }

    public int IdPedido { get; set; }

    public DateOnly FechaPedido { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<ComprasRealizada> ComprasRealizada { get; set; } = new List<ComprasRealizada>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<ItemsPedido> ItemsPedidos { get; set; } = new List<ItemsPedido>();
}
