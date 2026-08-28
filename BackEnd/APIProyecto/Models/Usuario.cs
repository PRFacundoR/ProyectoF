using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual Role IdRolNavigation { get; set; } = null!;

    public virtual Empleado IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<OrdenesPago> OrdenesPagoIdAutorizaNavigations { get; set; } = new List<OrdenesPago>();

    public virtual ICollection<OrdenesPago> OrdenesPagoIdUsuarioNavigations { get; set; } = new List<OrdenesPago>();

    public virtual ICollection<RecuperacionPassword> RecuperacionPasswords { get; set; } = new List<RecuperacionPassword>();
}
