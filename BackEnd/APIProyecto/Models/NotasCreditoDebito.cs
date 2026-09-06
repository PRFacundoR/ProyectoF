using System;
using System.Collections.Generic;

namespace APIProyecto.Models;

public partial class NotasCreditoDebito
{
    public int IdNota { get; set; }

    public int IdFactura { get; set; }

    public string? TipoNota { get; set; }

    public string NroComprobante { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public string? Motivo { get; set; }

    public decimal Monto { get; set; }

    public string? ArchivoAdjunto { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual ICollection<MovimientosCc> MovimientosCcs { get; set; } = new List<MovimientosCc>();
}
