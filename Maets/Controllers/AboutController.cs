using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Maets.Models;

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
