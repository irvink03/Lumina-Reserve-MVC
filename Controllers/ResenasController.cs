using Microsoft.AspNetCore.Mvc;

namespace Lumina_Reserve_MVC.Controllers
{
    public class ResenasController : Controller
    {
        // CAMBIO AQUÍ: Cambiamos Index() por Resenas() para que conecte con Resenas.cshtml
        public IActionResult Resenas()
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