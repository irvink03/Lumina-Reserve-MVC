using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Lumina_Reserve_MVC.Models;

namespace Lumina_Reserve_MVC.Controllers
{
    public class CuentaController : Controller
    {
        private readonly LuminaDbContext _context;
        private readonly IConfiguration _configuration;

        public CuentaController(LuminaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Cuenta/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Cuenta/Registro
        [HttpPost]
        public IActionResult Registro(string nombreCompleto, string correo, string telefono, string password)
        {
            if (string.IsNullOrEmpty(nombreCompleto) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Por favor completa todos los campos obligatorios.";
                return View();
            }

            bool correoExiste = _context.Usuarios.Any(u => u.Correo == correo);
            if (correoExiste)
            {
                TempData["Error"] = "El correo electrónico ya se encuentra registrado.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                NombreCompleto = nombreCompleto,
                Correo = correo,
                Telefono = telefono,
                PasswordHash = password,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            TempData["Exito"] = "¡Cuenta creada correctamente! Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // GET: /Cuenta/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        public IActionResult Login(string correo, string password, string codigoBot)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo && u.PasswordHash == password);
            if (usuario == null)
            {
                TempData["Error"] = "Correo electrónico o contraseña incorrectos.";
                return View();
            }

            // Validación del código de Telegram
            var codigoValido = _context.CodigosVerificacionBots
                .FirstOrDefault(c => c.UsuarioId == usuario.Id 
                                  && c.Codigo == codigoBot 
                                  && c.Usado == false 
                                  && c.FechaExpiracion > DateTime.Now);

            if (codigoValido == null)
            {
                TempData["Error"] = "El código de verificación de Telegram es inválido o ha expirado.";
                return View();
            }

            codigoValido.Usado = true;
            _context.Update(codigoValido);
            _context.SaveChanges();

            // Verificamos si el correo coincide con el administrador configurado en el appsettings.json
            string correoAdminOficial = _configuration["AdminSettings:CorreoAdmin"] ?? "";
            string rol = (!string.IsNullOrEmpty(correoAdminOficial) && usuario.Correo.Trim().ToLower() == correoAdminOficial.ToLower()) 
                ? "Administrador" 
                : "Cliente";

            // Guardar datos en la Sesión
            HttpContext.Session.SetString("UsuarioNombre", usuario.NombreCompleto);
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioRol", rol); 

            TempData["Exito"] = $"¡Bienvenido, {usuario.NombreCompleto}!";

            // Redirección inteligente según el rol
            if (rol == "Administrador")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("AgendarCita", "Citas");
            }
        }

        // GET: /Cuenta/Perfil
        public IActionResult Perfil()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login");
            }

            var usuario = _context.Usuarios.Find(usuarioId);
            if (usuario == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            ViewBag.Nombre = usuario.NombreCompleto;
            ViewBag.Telefono = usuario.Telefono;
            ViewBag.Correo = usuario.Correo;
            ViewBag.MiembroDesde = usuario.FechaRegistro?.ToString("dd/MM/yyyy");
            return View(usuario);
        }

        // GET: /Cuenta/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}