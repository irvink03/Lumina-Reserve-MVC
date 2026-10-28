using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class CodigosVerificacionBot
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Codigo { get; set; } = null!;

    public DateTime? FechaGeneracion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public bool? Usado { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
