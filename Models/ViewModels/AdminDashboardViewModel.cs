using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Models.ViewModels
{
    // Clase principal que agrupa todo para enviarlo a la vista
    public class AdminDashboardViewModel
    {
        public decimal IngresosHoy { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasPendientes { get; set; }
        public int NuevosRegistros { get; set; }

        public List<CitaViewModel> AgendaActiva { get; set; } = new List<CitaViewModel>();
        public List<RendimientoBarberoViewModel> RendimientoEquipo { get; set; } = new List<RendimientoBarberoViewModel>();
        public List<PagoViewModel> UltimosPagos { get; set; } = new List<PagoViewModel>();
    }

    // Sub-clases para las listas y tablas
    public class CitaViewModel
    {
        public string Hora { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public string Barbero { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class RendimientoBarberoViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Ingresos { get; set; }
        public int Cortes { get; set; }
        public int Porcentaje { get; set; }
    }

    public class PagoViewModel
    {
        public string Cliente { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public string Barbero { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}