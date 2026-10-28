using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Pago
{
    public int Id { get; set; }

    public int CitaId { get; set; }

    public string MetodoPago { get; set; } = null!;

    public decimal MontoTotal { get; set; }

    public decimal? DescuentoAplicado { get; set; }

    public DateTime? FechaPago { get; set; }

    public virtual Cita Cita { get; set; } = null!;
}
