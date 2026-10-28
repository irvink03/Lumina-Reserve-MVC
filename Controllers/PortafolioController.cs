using Microsoft.AspNetCore.Mvc;

namespace Lumina_Reserve_MVC.Controllers
{
    public class PortafolioController : Controller
    {
        public IActionResult Portafolio() // Nota: Puedes llamarlo simplemente Index() si prefieres que la URL sea más corta
        {
            return View();
        }
    }
}