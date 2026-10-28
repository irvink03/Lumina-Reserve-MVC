using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Sucursale
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string? Telefono { get; set; }

    public TimeOnly? HoraApertura { get; set; }

    public TimeOnly? HoraCierre { get; set; }

    public bool? Activa { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
