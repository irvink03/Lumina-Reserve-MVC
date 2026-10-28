using Microsoft.AspNetCore.Mvc;

namespace Lumina_Reserve_MVC.Controllers
{
    public class NosotrosController : Controller
    {
        public IActionResult NuestroEquipo()
        {
            return View();
        }

        public IActionResult Ubicacion()
        {
            return View();
        }
    }
}