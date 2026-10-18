using Microsoft.AspNetCore.Mvc;

namespace Lumina_Reserve_MVC.Controllers
{
    public class ResenasController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult GuardarResena(string nombreCliente, int idBarbero, int calificacion, string comentario)
        {
            return Json(new { success = true, message = "Reseña guardada." });
        }
    }
}