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
using Maets.Models.ExternalData;
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
    private readonly DocxDataService _docxDataService;
    private readonly DataTransformationService _transformationService;

    public AppsController(MaetsDbContext context, IFileWriteService fileWriteService, ILabelsService labelsService, IMapper mapper, DataTransformationService transformationService, ExcelDataService excelDataService, IAppsService appsService, DocxDataService docxDataService)
    {
        _context = context;
        _fileWriteService = fileWriteService;
        _labelsService = labelsService;
        _mapper = mapper;
        _transformationService = transformationService;
        _excelDataService = excelDataService;
        _appsService = appsService;
        _docxDataService = docxDataService;
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

    [HttpGet, ActionName("ExportSpreadsheet")]
    public async Task<IActionResult> ExportSpreadsheet(IEnumerable<Guid>? selectedAppIds = null)
    {
        var table = await GetExportTable(selectedAppIds);
        
        using var memoryStream = new MemoryStream();
        _excelDataService.ExportSpreadsheetToStream(memoryStream, table);
        await memoryStream.FlushAsync();
        
        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"AppsExport-{DateTime.Now}.xlsx"
        };
    }
    
    [HttpGet, ActionName("ExportDocument")]
    public async Task<IActionResult> ExportDocument(IEnumerable<Guid>? selectedAppIds = null)
    {
        var table = await GetExportTable(selectedAppIds);

        using var memoryStream = new MemoryStream();
        _docxDataService.ExportDocumentToStream(memoryStream, table);
        await memoryStream.FlushAsync();
        
        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            FileDownloadName = $"AppsExport-{DateTime.Now}.docx"
        };
    }

    [HttpPost, ActionName("ImportSpreadsheet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportFromSpreadsheet(IFormFile spreadsheetFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var fileStream = spreadsheetFile.OpenReadStream();
            var tableData = _excelDataService.ImportFromSpreadsheetStream(fileStream);
            var apps = _transformationService.FromTableData<AppExternalDto>(tableData);

            await AddImportedApps(apps);
        }
        catch (Exception)
        {
            ModelState.AddModelError("spreadsheetFile", "Invalid spreadsheet");
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost, ActionName("ImportDocument")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportFromDocument(IFormFile documentFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var fileStream = documentFile.OpenReadStream();
            var tableData = _docxDataService.ImportFromDocumentStream(fileStream);
            var apps = _transformationService.FromTableData<AppExternalDto>(tableData);

            await AddImportedApps(apps);
        }
        catch (Exception)
        {
            ModelState.AddModelError("documentFile", "Invalid spreadsheet");
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task AddImportedApps(IEnumerable<AppExternalDto> apps)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        foreach (var app in apps)
        {
            await _appsService.CreateApp(app);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    
    private async Task<CommonTable> GetExportTable(IEnumerable<Guid>? appIds = null)
    {
        var apps = await _context.Apps
            .Include(x => x.Developers)
            .Include(x => x.Publisher)
            .Include(x => x.Labels)
            .WhereIf(x => appIds!.Contains(x.Id), appIds is not null && appIds.Any())
            .ToListAsync();

        var appDtos = _mapper.Map<List<AppExternalDto>>(apps);
        return _transformationService.ToTableData(appDtos);
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
