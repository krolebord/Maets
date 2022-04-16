using Microsoft.AspNetCore.Mvc;

namespace Maets.Controllers;

public class AboutController : MaetsController
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
