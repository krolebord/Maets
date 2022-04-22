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
    public async Task<IActionResult> Create()
    {
        await LoadViewData();
        return View();
    }

    // POST: Companies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyWriteDto companyDto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(companyDto);
        }

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = companyDto.Name,
            Description = companyDto.Description ?? string.Empty
        };

        if (companyDto.Photo is not null)
        {
            var photoKey = BuildCompanyPhotoKey(company);
            var photoFile = await _fileWriteService.UploadFileAsync(photoKey, companyDto.Photo.OpenReadStream());
            company.Photo = photoFile;
        }

        company.Employees = await _context.Users
            .Where(x => companyDto.EmployeeIds.Contains(x.Id))
            .ToListAsync();

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

        var company = await _context.Companies
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        await LoadViewData();
        return View(new CompanyWriteDto
        {
            Name = company.Name,
            Description = company.Description,
            EmployeeIds = company.Employees.Select(x => x.Id)
        });
    }

    // POST: Companies/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyWriteDto companyDto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(companyDto);
        }

        var company = await _context.Companies
            .Include(x => x.Photo)
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company is null)
        {
            return NotFound();
        }

        company.Name = companyDto.Name;
        company.Description = companyDto.Description ?? string.Empty;

        if (companyDto.Photo is not null)
        {
            if (company.Photo is not null)
            {
                await _fileWriteService.DeleteFileAsync(company.Photo);
            }

            var photoKey = BuildCompanyPhotoKey(company);
            company.Photo = await _fileWriteService.UploadFileAsync(photoKey, companyDto.Photo.OpenReadStream());
        }
        
        company.Employees = await _context.Users
            .Where(x => companyDto.EmployeeIds.Contains(x.Id))
            .ToListAsync();

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

    private async Task LoadViewData()
    {
        var users = await _context.Users
            .Select(x => new
            {
                Id = x.Id,
                UserName = x.UserName
            })
            .OrderBy(x => x.UserName)
            .ToListAsync();

        ViewData["Users"] = new SelectList(users, "Id", "UserName");
    }
    
    private string BuildCompanyPhotoKey(Company company)
    {
        return $"company-photos/{company.Id}-{Guid.NewGuid()}.png";
    }
}
