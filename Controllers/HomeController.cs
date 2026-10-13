using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lumina_Reserve_MVC.Models;

namespace Lumina_Reserve_MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // Nuevo controlador para la página de reservas
    public IActionResult AgendarCita()
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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
