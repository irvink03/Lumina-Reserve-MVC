using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class CitaCorte
{
    public int CitaId { get; set; }

    public int CorteId { get; set; }

    public decimal PrecioCobrado { get; set; }

    public virtual Cita Cita { get; set; } = null!;

    public virtual Corte Corte { get; set; } = null!;
}
