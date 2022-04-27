using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Constants;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Maets.Services.Apps;
using Maets.Services.ExternalData;
using Maets.Services.Files;
using Maets.Services.Labels;
using Microsoft.AspNetCore.Authorization;

namespace Maets.Controllers;

[Authorize(Roles = RoleNames.AdminOrModerator)]
public class AppsController : MaetsController
{
    private readonly MaetsDbContext _context;
    private readonly IAppsService _appsService;
    private readonly IFileWriteService _fileWriteService;
    private readonly ILabelsService _labelsService;
    private readonly IMapper _mapper;
    private readonly ExcelDataService _excelDataService;
    private readonly DataTransformationService _transformationService;

    public AppsController(MaetsDbContext context, IFileWriteService fileWriteService, ILabelsService labelsService, IMapper mapper, DataTransformationService transformationService, ExcelDataService excelDataService, IAppsService appsService)
    {
        _context = context;
        _fileWriteService = fileWriteService;
        _labelsService = labelsService;
        _mapper = mapper;
        _transformationService = transformationService;
        _excelDataService = excelDataService;
        _appsService = appsService;
    }

    // GET: Apps
    public async Task<IActionResult> Index()
    {
        var apps = await _context.Apps
            .Include(a => a.Labels)
            .Include(a => a.Developers)
            .Include(a => a.Publisher)
            .ToListAsync();

        var appDtos = _mapper.Map<List<AppTableDto>>(apps);
        
        ViewData["SelectableApps"] = new SelectList(apps, "Id", "Title");
        
        return View(appDtos);
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
    public async Task<IActionResult> Create()
    {
        await LoadViewData();
        return View();
    }

    // POST: Apps/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppCreateDto appDto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(appDto);
        }

        if (await _context.Apps.AnyAsync(x => x.Title == appDto.Title))
        {
            ModelState.AddModelError("title", "App with the same title already exists");
            await LoadViewData();
            return View(appDto);
        }

        await _appsService.CreateApp(appDto);
        
        return RedirectToAction(nameof(Index));
    }

    // GET: Apps/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var app = await _context.Apps
            .Include(x => x.Publisher)
            .Include(x => x.Developers)
            .Include(x => x.Labels)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (app == null)
        {
            return NotFound();
        }

        var appDto = _mapper.Map<AppEditDto>(app);
        
        await LoadViewData();
        return View(appDto);
    }

    // POST: Apps/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AppEditDto appDto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(appDto);
        }
        
        var app = await _context.Apps
            .Include(x => x.Publisher)
            .Include(x => x.Developers)
            .Include(x => x.Labels)
            .Include(x => x.MainImage)
            .Include(x => x.Screenshots)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (app is null)
        {
            return NotFound();
        }

        _mapper.Map(appDto, app);

        var publisher = await _context.Companies.FirstOrDefaultAsync(x => x.Id == appDto.PublisherId);
        if (publisher is not null)
        {
            app.Publisher = publisher;
        }

        var developers = await _context.Companies
            .Where(x => appDto.DeveloperIds.Contains(x.Id))
            .ToListAsync();
        app.Developers = developers;

        app.Labels = (await _labelsService.GetOrAddLabelsByNames(appDto.Labels)).ToList();

        if (appDto.MainImage is not null)
        {
            if (app.MainImage is not null)
            {
                await _fileWriteService.DeleteFileAsync(app.MainImage);
            }
            var mainImageKey = _appsService.BuildAppScreenshotKey(app.Id);
            app.MainImage = await _fileWriteService.UploadFileAsync(mainImageKey, appDto.MainImage.OpenReadStream());
        }

        if (appDto.Screenshots is not null)
        {
            foreach (var oldScreenshot in app.Screenshots)
            {
                await _fileWriteService.DeleteFileAsync(oldScreenshot);
            }
            app.Screenshots.Clear();
            foreach (var screenshot in appDto.Screenshots)
            {
                var fileKey = _appsService.BuildAppScreenshotKey(app.Id);
                var file = await _fileWriteService.UploadFileAsync(fileKey, screenshot.OpenReadStream());
                app.Screenshots.Add(file);
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
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

        if (app is not null)
        {
            _context.Apps.Remove(app);
            await _context.SaveChangesAsync();    
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet, ActionName("Export")]
    public async Task<IActionResult> ExportApps(IEnumerable<Guid>? selectedAppIds = null)
    {
        var apps = await _context.Apps
            .Include(x => x.Developers)
            .Include(x => x.Publisher)
            .Include(x => x.Labels)
            .WhereIf(x => selectedAppIds!.Contains(x.Id), selectedAppIds is not null && selectedAppIds.Any())
            .ToListAsync();

        var appDtos = _mapper.Map<List<AppExternalDto>>(apps);
        var table = _transformationService.ToTableData(appDtos);
        
        using var workbook = _excelDataService.ExportWorkbook(table);
        
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        await memoryStream.FlushAsync();
        
        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "AppsExport.xlsx"
        };
    }

    [HttpPost, ActionName("Import")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportApps(IFormFile spreadsheetFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var fileStream = spreadsheetFile.OpenReadStream();
            using var workbook = new XLWorkbook(fileStream, XLEventTracking.Disabled);

            var tableData = _excelDataService.ImportFromWorkbook(workbook);
            var apps = _transformationService.FromTableData<AppExternalDto>(tableData);
            await using var transaction = await _context.Database.BeginTransactionAsync();

            foreach (var app in apps)
            {
                await _appsService.CreateApp(app);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            ModelState.AddModelError("spreadsheetFile", "Invalid spreadsheet");
            return RedirectToAction(nameof(Index));
        }
        
        
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadViewData()
    {
        var companies = await _context.Companies
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var labels = await _context.Labels
            .Select(x => new
            {
                Name = x.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync();
        
        ViewData["Companies"] = new SelectList(companies, "Id", "Name");
        ViewData["Labels"] = new SelectList(labels, "Name", "Name");
    }
}
