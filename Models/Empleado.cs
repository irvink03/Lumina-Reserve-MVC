using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int SucursalId { get; set; }

    public string? Especialidad { get; set; }

    public decimal? PorcentajeComision { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    public virtual Sucursale Sucursal { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
