using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
// ¡NUEVO!: Esta es la línea que soluciona el error
using Lumina_Reserve_MVC.Models;

namespace Lumina_Reserve_MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // Recibe al barbero y el corte seleccionado desde el Portafolio
    public IActionResult AgendarCita(string barbero, string corte)
    {
        ViewBag.Barbero = string.IsNullOrEmpty(barbero) ? "Cualquier barbero" : barbero;
        ViewBag.Corte = corte; 
        return View();
    }

    public IActionResult NuestroEquipo()
    {
        return View();
    }

    // Habilita la página del Portafolio
    public IActionResult Portafolio(string barbero)
    {
        ViewBag.Barbero = string.IsNullOrEmpty(barbero) ? "Lumina Reserve" : barbero;
        return View();
    }

    // Nueva vista para Ubicación y Contacto (Normalización)
    public IActionResult Ubicacion()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // El sistema ya sabe que "ErrorViewModel" está en la carpeta Models
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}