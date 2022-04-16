using System.Diagnostics;
using Maets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Maets.Controllers;

public abstract class MaetsController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
