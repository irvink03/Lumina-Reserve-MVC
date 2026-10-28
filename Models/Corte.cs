using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Corte
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string Categoria { get; set; } = null!;

    public decimal Precio { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<CitaCorte> CitaCortes { get; set; } = new List<CitaCorte>();
}
