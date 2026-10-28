using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Notificacione
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int? CitaId { get; set; }

    public string Tipo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public bool? Enviado { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public virtual Cita? Cita { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
