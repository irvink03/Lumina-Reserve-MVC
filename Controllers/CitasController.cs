using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Lumina_Reserve_MVC.Models;
using Lumina_Reserve_MVC.Services;

namespace Lumina_Reserve_MVC.Controllers
{
    public class CitasController : Controller
    {
        private readonly LuminaDbContext _context;
        private readonly TelegramNotificador _telegram;

        public CitasController(LuminaDbContext context, TelegramNotificador telegram)
        {
            _context = context;
            _telegram = telegram;
        }

        //  Carga la pantalla con los datos reales de SQL
        public IActionResult AgendarCita()
        {
            ViewBag.Sucursales = _context.Sucursales.Where(s => s.Activa == true).ToList();
            ViewBag.Cortes = _context.Cortes.Where(c => c.Activo == true).ToList();

            ViewBag.Barberos = _context.Empleados
                .Include(e => e.Usuario)
                .Include(e => e.Sucursal)
                .Where(e => e.Activo == true)
                .ToList();

            return View();
        }

        //  Guarda la cita llamando al procedimiento almacenado y avisa por Telegram
        [HttpPost]
        public async Task<IActionResult> ConfirmarReserva(int SucursalId, int? BarberoId, List<int> CortesSeleccionados, DateOnly FechaCita, TimeOnly HoraCita, string MetodoPago)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Error"] = "Inicia sesión para agendar tu cita.";
                return RedirectToAction("Login", "Cuenta");
            }

            if (CortesSeleccionados == null || !CortesSeleccionados.Any())
            {
                TempData["Error"] = "Debes seleccionar al menos un servicio.";
                return RedirectToAction("AgendarCita");
            }

            try
            {
                var pCitaId = new SqlParameter("@CitaId", SqlDbType.Int) { Direction = ParameterDirection.Output };

                _context.Database.ExecuteSqlRaw(
                    "EXEC dbo.sp_AgendarCita @ClienteId, @SucursalId, @BarberoId, @FechaCita, @HoraCita, @MetodoPago, @CortesIds, @CitaId OUTPUT",
                    new SqlParameter("@ClienteId", SqlDbType.Int) { Value = usuarioId.Value },
                    new SqlParameter("@SucursalId", SqlDbType.Int) { Value = SucursalId },
                    new SqlParameter("@BarberoId", SqlDbType.Int) { Value = (object?)BarberoId ?? DBNull.Value },
                    new SqlParameter("@FechaCita", SqlDbType.Date) { Value = FechaCita.ToDateTime(TimeOnly.MinValue) },
                    new SqlParameter("@HoraCita", SqlDbType.Time) { Value = HoraCita.ToTimeSpan() },
                    new SqlParameter("@MetodoPago", SqlDbType.NVarChar, 100) { Value = MetodoPago },
                    new SqlParameter("@CortesIds", SqlDbType.NVarChar, -1) { Value = string.Join(",", CortesSeleccionados) },
                    pCitaId);

                string mensajeExito = "¡Tu cita ha sido agendada correctamente!";

                // La cita ya quedó guardada; el aviso por Telegram es un extra
                if (pCitaId.Value is int citaId)
                {
                    bool avisoEnviado = await NotificarCitaAsync(citaId);
                    mensajeExito += avisoEnviado
                        ? " Te enviamos los detalles por Telegram. 📩"
                        : " Para recibir avisos por Telegram, abre nuestro bot y envíale tu correo.";
                }

                TempData["Exito"] = mensajeExito;
                return RedirectToAction("AgendarCita");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al agendar la cita: " + ex.Message;
                return RedirectToAction("AgendarCita");
            }
        }

        // GET: /Citas/Historial  -> citas del usuario que inició sesión
        public IActionResult Historial()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var citas = _context.Citas
                .Include(c => c.Sucursal)
                .Include(c => c.Barbero).ThenInclude(b => b.Usuario)
                .Include(c => c.CitaCortes).ThenInclude(cc => cc.Corte)
                .Include(c => c.Pago)
                .Where(c => c.ClienteId == usuarioId.Value)
                .OrderByDescending(c => c.FechaCita)
                .ThenByDescending(c => c.HoraCita)
                .ToList();

            return View(citas);
        }

        // Envía el aviso por Telegram y lo registra en la tabla Notificaciones.
        // Nunca lanza excepciones: si algo falla, simplemente devuelve false.
        private async Task<bool> NotificarCitaAsync(int citaId)
        {
            try
            {
                var cita = _context.Citas
                    .AsNoTracking()
                    .Include(c => c.Cliente)
                    .Include(c => c.Sucursal)
                    .Include(c => c.Barbero).ThenInclude(b => b.Usuario)
                    .Include(c => c.CitaCortes).ThenInclude(cc => cc.Corte)
                    .Include(c => c.Pago)
                    .FirstOrDefault(c => c.Id == citaId);

                if (cita == null || cita.Cliente.TelegramChatId == null)
                {
                    return false;
                }

                long chatId = cita.Cliente.TelegramChatId.Value;
                bool enviado = await _telegram.EnviarAsync(chatId, TelegramNotificador.ConstruirMensajeCita(cita));

                // Bitácora (si falla, no afecta el aviso)
                try
                {
                    _context.Notificaciones.Add(new Notificacione
                    {
                        UsuarioId = cita.ClienteId,
                        CitaId = cita.Id,
                        Tipo = "CitaAgendada",
                        Mensaje = $"Cita agendada para el {cita.FechaCita:dd/MM/yyyy} a las {cita.HoraCita:HH:mm}",
                        Enviado = enviado,
                        FechaEnvio = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // sin bitácora, pero el aviso ya salió
                }

                return enviado;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}