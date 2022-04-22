using Microsoft.AspNetCore.Mvc;

namespace Maets.Controllers;

public class StoreController : MaetsController
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Search()
    {
        return View();
    }
}
