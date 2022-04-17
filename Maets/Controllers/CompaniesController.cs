using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Constants;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.Companies;
using Maets.Services.Files;
using Microsoft.AspNetCore.Authorization;

namespace Maets.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class CompaniesController : MaetsController
{
    private readonly MaetsDbContext _context;
    private readonly IFileReadService _fileReadService;
    private readonly IFileWriteService _fileWriteService;
    private readonly IMapper _mapper;

    public CompaniesController(MaetsDbContext context, IFileReadService fileReadService, IMapper mapper, IFileWriteService fileWriteService)
    {
        _context = context;
        _fileReadService = fileReadService;
        _mapper = mapper;
        _fileWriteService = fileWriteService;
    }

    // GET: Companies
    public async Task<IActionResult> Index()
    {
        var companies = await GetCompanyReadDtoQuery()
            .ToListAsync();

        return View(companies);
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
        return View();
    }

    // POST: Companies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyWriteDto companyDto)
    {
        if (!ModelState.IsValid)
            return View(companyDto);

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = companyDto.Name,
            Description = companyDto.Description
        };

        if (companyDto.Photo is not null)
        {
            var photoKey = $"company-photo-{Guid.NewGuid()}.png";
            var photoFile = await _fileWriteService.UploadFileAsync(photoKey, companyDto.Photo.OpenReadStream());
            company.Photo = photoFile;
        }

        _context.Add(company);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

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

        return View(new CompanyWriteDto
        {
            Name = company.Name,
            Description = company.Description
        });
    }

    // POST: Companies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyWriteDto companyDto)
    {
        if (!ModelState.IsValid)
            return View(companyDto);

        var company = await _context.Companies
            .Include(x => x.Photo)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company is null)
        {
            return NotFound();
        }

        company.Name = companyDto.Name;
        company.Description = companyDto.Description;

        if (companyDto.Photo is not null)
        {
            if (company.Photo is not null)
            {
                await _fileWriteService.DeleteFileAsync(company.Photo);
            }

            var photoKey = $"company-photo-{Guid.NewGuid()}.png";
            company.Photo = await _fileWriteService.UploadFileAsync(photoKey, companyDto.Photo.OpenReadStream());
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Companies/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var companyDto = await GetCompanyReadDtoQuery(id)
            .FirstOrDefaultAsync();
        if (companyDto == null)
        {
            return NotFound();
        }

        return View(companyDto);
    }

    // POST: Companies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is not null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<CompanyReadDto> GetCompanyReadDtoQuery(Guid? id = null)
    {
        IQueryable<Company> query = _context.Companies
            .Include(c => c.Photo);

        if (id is not null)
        {
            query = query.Where(x => x.Id == id.Value);
        }

        return query.Select(x => new CompanyReadDto(
            x.Id,
            x.Name,
            x.Description,
            _fileReadService.ImageUrlOrDefault(x.Photo)
        )
        {
            EmployeesCount = x.Employees.Count,
            DevelopedAppsCount = x.DevelopedApps.Count,
            PublishedAppsCount = x.PublishedApps.Count
        });
    }
}
