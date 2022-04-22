using Maets.Data;
using Maets.Domain.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maets.Controllers;

public class AboutController : MaetsController
{
    private readonly MaetsDbContext _context;

    public AboutController(MaetsDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var maetsAppId = await _context.Apps
            .Where(x => x.Title == AppsSeedData.DefaultAppTitle)
            .Select(x => x.Id)
            .FirstAsync();
        return RedirectToPage("/store/app", new { id = maetsAppId });
    }

    public IActionResult Home()
    {
        return RedirectToPage("/store/home");
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
