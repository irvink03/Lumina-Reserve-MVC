using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lumina_Reserve_MVC.Models; // Asegúrate de que este sea el nombre correcto de tu proyecto

namespace Lumina_Reserve_MVC.Controllers
{
    public class CitasController : Controller
    {
        private readonly LuminaDbContext _context;

        // Inyectamos el puente de la base de datos
        public CitasController(LuminaDbContext context)
        {
            _context = context;
        }

        // 1. GET: Carga la pantalla con los datos reales de SQL
        public IActionResult AgendarCita()
        {
            // Mandamos los catálogos a la vista
            ViewBag.Sucursales = _context.Sucursales.Where(s => s.Activa == true).ToList();
            ViewBag.Cortes = _context.Cortes.Where(c => c.Activo == true).ToList();
            
            // Incluimos la tabla Usuario para poder mostrar el nombre del barbero
            ViewBag.Barberos = _context.Empleados
                .Include(e => e.Usuario)
                .Where(e => e.Activo == true)
                .ToList();

            return View();
        }

        // 2. POST: Guarda la cita cuando el cliente presiona "Confirmar reserva"
        [HttpPost]
        public IActionResult ConfirmarReserva(int SucursalId, int BarberoId, List<int> CortesSeleccionados, DateOnly FechaCita, TimeOnly HoraCita, string MetodoPago)
        {
            // Validar que seleccionó al menos un servicio
            if (CortesSeleccionados == null || !CortesSeleccionados.Any())
            {
                TempData["Error"] = "Debes seleccionar al menos un servicio.";
                return RedirectToAction("AgendarCita");
            }

            // Usamos una transacción por si algo falla, no se guarde información a medias
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // A. Crear la Cita (Temporalmente ponemos ClienteId = 1 hasta que hagamos el Login)
                var nuevaCita = new Cita
                {
                    ClienteId = 1, // <-- OJO: esto lo cambiaremos cuando conectemos el inicio de sesión
                    BarberoId = BarberoId,
                    SucursalId = SucursalId,
                    FechaCita = FechaCita,
                    HoraCita = HoraCita,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now
                };
                
                _context.Citas.Add(nuevaCita);
                _context.SaveChanges(); // Guardamos para generar el ID de la cita

                decimal montoTotal = 0;

                // B. Guardar los cortes que seleccionó y sumar el total
                foreach (var corteId in CortesSeleccionados)
                {
                    var corteBd = _context.Cortes.Find(corteId);
                    if (corteBd != null)
                    {
                        var citaCorte = new CitaCorte
                        {
                            CitaId = nuevaCita.Id,
                            CorteId = corteBd.Id,
                            PrecioCobrado = corteBd.Precio
                        };
                        _context.CitaCortes.Add(citaCorte);
                        montoTotal += corteBd.Precio;
                    }
                }
                _context.SaveChanges();

                // C. Generar el registro del Pago
                var nuevoPago = new Pago
                {
                    CitaId = nuevaCita.Id,
                    MetodoPago = MetodoPago,
                    MontoTotal = montoTotal,
                    DescuentoAplicado = 0,
                    FechaPago = DateTime.Now
                };
                _context.Pagos.Add(nuevoPago);
                _context.SaveChanges();

                // Confirmamos la transacción
                transaction.Commit();

                TempData["Exito"] = "¡Tu cita ha sido agendada correctamente!";
                return RedirectToAction("AgendarCita");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["Error"] = "Ocurrió un error al agendar la cita: " + ex.Message;
                return RedirectToAction("AgendarCita");
            }
        }
    }
}