using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Resena
{
    public int Id { get; set; }

    public int CitaId { get; set; }

    public int ClienteId { get; set; }

    public int BarberoId { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public bool? Aprobada { get; set; }

    public DateTime? FechaResena { get; set; }

    public virtual Empleado Barbero { get; set; } = null!;

    public virtual Cita Cita { get; set; } = null!;

    public virtual Usuario Cliente { get; set; } = null!;
}
