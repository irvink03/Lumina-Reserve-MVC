using System;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models;

public partial class Gasto
{
    public int Id { get; set; }

    public int SucursalId { get; set; }

    public int RegistradoPorId { get; set; }

    public string Categoria { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime? FechaGasto { get; set; }

    public string? ComprobanteUrl { get; set; }

    public virtual Usuario RegistradoPor { get; set; } = null!;

    public virtual Sucursale Sucursal { get; set; } = null!;
}
