#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Entities;

namespace Maets.Controllers;

public class AppsController : MaetsController
{
    private readonly MaetsDbContext _context;

    public AppsController(MaetsDbContext context)
    {
        _context = context;
    }

    // GET: Apps
    public async Task<IActionResult> Index()
    {
        var maetsDbContext = _context.Apps.Include(a => a.Publisher);
        return View(await maetsDbContext.ToListAsync());
    }

    // GET: Apps/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var app = await _context.Apps
            .Include(a => a.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (app == null)
        {
            return NotFound();
        }

        return View(app);
    }

    // GET: Apps/Create
    public IActionResult Create()
    {
        ViewData["PublisherId"] = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    // POST: Apps/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,ReleaseDate,Price,PublisherId")] App app)
    {
        if (ModelState.IsValid)
        {
            app.Id = Guid.NewGuid();
            _context.Add(app);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["PublisherId"] = new SelectList(_context.Companies, "Id", "Id", app.PublisherId);
        return View(app);
    }

    // GET: Apps/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var app = await _context.Apps.FindAsync(id);
        if (app == null)
        {
            return NotFound();
        }
        ViewData["PublisherId"] = new SelectList(_context.Companies, "Id", "Id", app.PublisherId);
        return View(app);
    }

    // POST: Apps/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,ReleaseDate,Price,PublisherId")] App app)
    {
        if (id != app.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(app);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppExists(app.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["PublisherId"] = new SelectList(_context.Companies, "Id", "Id", app.PublisherId);
        return View(app);
    }

    // GET: Apps/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var app = await _context.Apps
            .Include(a => a.Publisher)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (app == null)
        {
            return NotFound();
        }

        return View(app);
    }

    // POST: Apps/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var app = await _context.Apps.FindAsync(id);
        _context.Apps.Remove(app);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AppExists(Guid id)
    {
        return _context.Apps.Any(e => e.Id == id);
    }
}
