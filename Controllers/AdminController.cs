using Microsoft.AspNetCore.Mvc;
using Lumina_Reserve_MVC.Models;
using Lumina_Reserve_MVC.Models.ViewModels;
using System.Collections.Generic;

namespace Lumina_Reserve_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly LuminaDbContext _context;

        // Inyectamos la conexión a la base de datos
        public AdminController(LuminaDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            /* 
               CUANDO TUS TABLAS ESTÉN LISTAS, REEMPLAZARÁS LOS VALORES MANUALES CON CONSULTAS REALES. EJEMPLO:
               var ingresosReales = _context.Pagos.Where(p => p.Fecha.Date == DateTime.Today).Sum(p => p.Total);
               var citasReales = _context.Citas.Count(c => c.Fecha.Date == DateTime.Today && c.Estado == "Completada");
            */

            // Construimos el modelo con datos estructurados listos para iterar
            var modelo = new AdminDashboardViewModel
            {
                IngresosHoy = 4850.00m,
                CitasCompletadas = 14,
                CitasPendientes = 2,
                NuevosRegistros = 3,
                
                AgendaActiva = new List<CitaViewModel>
                {
                    new CitaViewModel { Hora = "16:00", Cliente = "Carlos Martínez", Servicio = "Skin Fade + Barba", Barbero = "Irvin Chavarría", Estado = "En Curso" },
                    new CitaViewModel { Hora = "16:45", Cliente = "Luis Sánchez", Servicio = "Corte Brócoli", Barbero = "Diego Silva", Estado = "Espera" }
                },
                
                RendimientoEquipo = new List<RendimientoBarberoViewModel>
                {
                    new RendimientoBarberoViewModel { Nombre = "Irvin Chavarría", Ingresos = 2100m, Cortes = 6, Porcentaje = 70 },
                    new RendimientoBarberoViewModel { Nombre = "Fernando Rodríguez", Ingresos = 1500m, Cortes = 4, Porcentaje = 50 },
                    new RendimientoBarberoViewModel { Nombre = "Diego Silva", Ingresos = 1250m, Cortes = 4, Porcentaje = 40 }
                },
                
                UltimosPagos = new List<PagoViewModel>
                {
                    new PagoViewModel { Cliente = "Alejandro G.", Servicio = "Corte Brócoli + Facial", MetodoPago = "Transferencia", Barbero = "Irvin Chavarría", Total = 550.00m },
                    new PagoViewModel { Cliente = "Roberto M.", Servicio = "Skin Fade", MetodoPago = "Efectivo", Barbero = "Fernando R.", Total = 180.00m },
                    new PagoViewModel { Cliente = "Miguel S.", Servicio = "Cambio de Look Total", MetodoPago = "Transferencia", Barbero = "Diego Silva", Total = 750.00m }
                }
            };

            // Enviamos todo el paquete de datos a la vista
            return View(modelo);
        }
    }
}