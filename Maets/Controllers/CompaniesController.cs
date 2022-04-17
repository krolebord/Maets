using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Constants;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.Company;
using Maets.Services.Files;
using Microsoft.AspNetCore.Authorization;

namespace Maets.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class CompaniesController : MaetsController
{
    private readonly MaetsDbContext _context;
    private readonly IFileReadService _fileReadService;

    public CompaniesController(MaetsDbContext context, IFileReadService fileReadService)
    {
        _context = context;
        _fileReadService = fileReadService;
    }

    // GET: Companies
    public async Task<IActionResult> Index()
    {
        var companies = _context.Companies
            .Include(c => c.Photo)
            .ToList();

        return View(companies.Select(x => new CompanyReadDto(
            x.Id,
            x.Name,
            x.Description,
            _fileReadService.ImageUrlOrDefault(x.Photo)
        )));
    }

    // GET: Companies/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var company = await _context.Companies
            .Include(c => c.Photo)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        return View(company);
    }

    // GET: Companies/Create
    public IActionResult Create()
    {
        ViewData["PhotoId"] = new SelectList(_context.MediaFiles, "Id", "Id");
        return View();
    }

    // POST: Companies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,PhotoId")] Company company)
    {
        if (ModelState.IsValid)
        {
            company.Id = Guid.NewGuid();
            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["PhotoId"] = new SelectList(_context.MediaFiles, "Id", "Id", company.PhotoId);
        return View(company);
    }

    // GET: Companies/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return NotFound();
        }
        ViewData["PhotoId"] = new SelectList(_context.MediaFiles, "Id", "Id", company.PhotoId);
        return View(company);
    }

    // POST: Companies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,PhotoId")] Company company)
    {
        if (id != company.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(company);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(company.Id))
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
        ViewData["PhotoId"] = new SelectList(_context.MediaFiles, "Id", "Id", company.PhotoId);
        return View(company);
    }

    // GET: Companies/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var company = await _context.Companies
            .Include(c => c.Photo)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        return View(company);
    }

    // POST: Companies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CompanyExists(Guid id)
    {
        return _context.Companies.Any(e => e.Id == id);
    }
}
