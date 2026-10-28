using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Cita
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int BarberoId { get; set; }

    public int SucursalId { get; set; }

    public DateOnly FechaCita { get; set; }

    public TimeOnly HoraCita { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Empleado Barbero { get; set; } = null!;

    public virtual ICollection<CitaCorte> CitaCortes { get; set; } = new List<CitaCorte>();

    public virtual Usuario Cliente { get; set; } = null!;

    public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();

    public virtual Pago? Pago { get; set; }

    public virtual Resena? Resena { get; set; }

    public virtual Sucursale Sucursal { get; set; } = null!;
}
