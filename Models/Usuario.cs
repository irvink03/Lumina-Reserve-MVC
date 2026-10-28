using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<CodigosVerificacionBot> CodigosVerificacionBots { get; set; } = new List<CodigosVerificacionBot>();

    public virtual Empleado? Empleado { get; set; }

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();

    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    public virtual ICollection<Role> Rols { get; set; } = new List<Role>();
}
