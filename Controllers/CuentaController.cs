using Microsoft.AspNetCore.Mvc;

namespace Lumina_Reserve_MVC.Controllers
{
    public class CuentaController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        // Método que llama a Views/Cuenta/Perfil.cshtml
        public IActionResult Perfil()
        {
            ViewBag.Nombre = "Irving Pérez";
            ViewBag.Correo = "irving@lumina.com";
            ViewBag.Telefono = "961 123 4567";
            ViewBag.MiembroDesde = "Octubre 2026";
            
            return View();
        }
    }
}