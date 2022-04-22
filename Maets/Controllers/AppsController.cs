using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Models.Dtos.Apps;
using Maets.Services.Files;
using Maets.Services.Labels;

namespace Maets.Controllers;

public class AppsController : MaetsController
{
    private readonly MaetsDbContext _context;
    private readonly IFileReadService _fileReadService;
    private readonly IFileWriteService _fileWriteService;
    private readonly ILabelsService _labelsService;
    private readonly IMapper _mapper;

    public AppsController(MaetsDbContext context, IFileReadService fileReadService, IFileWriteService fileWriteService, ILabelsService labelsService, IMapper mapper)
    {
        _context = context;
        _fileReadService = fileReadService;
        _fileWriteService = fileWriteService;
        _labelsService = labelsService;
        _mapper = mapper;
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

        var app = new App
        {
            Id = Guid.NewGuid(),
            Title = appDto.Title,
            Description = appDto.Description,
            ReleaseDate = appDto.ReleaseDate,
            Price = appDto.Price
        };

        var mainImageKey = BuildAppScreenshotId(app.Id);
        app.MainImage = await _fileWriteService.UploadFileAsync(mainImageKey, appDto.MainImage.OpenReadStream());

        if (appDto.Screenshots is not null)
        {
            foreach (var screenshot in appDto.Screenshots)
            {
                var fileKey = BuildAppScreenshotId(app.Id);
                var file = await _fileWriteService.UploadFileAsync(fileKey, screenshot.OpenReadStream());
                app.Screenshots.Add(file);
            }
        }

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

        _context.Apps.Add(app);
        await _context.SaveChangesAsync();
        
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
            await _fileWriteService.DeleteFileAsync(app.MainImage!);
            var mainImageKey = BuildAppScreenshotId(app.Id);
            app.MainImage = await _fileWriteService.UploadFileAsync(mainImageKey, appDto.MainImage.OpenReadStream());
        }

        if (appDto.Screenshots is not null)
        {
            foreach (var oldScreenshot in app.Screenshots)
            {
                await _fileWriteService.DeleteFileAsync(oldScreenshot);
            }
            foreach (var screenshot in appDto.Screenshots)
            {
                app.Screenshots.Clear();
                var fileKey = BuildAppScreenshotId(app.Id);
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

    public string BuildAppScreenshotId(Guid appId)
    {
        return $"app-screenshot/{appId}/{Guid.NewGuid()}.png";
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

        ViewData["Companies"] = new SelectList(companies, "Id", "Name");

        var labels = await _context.Labels
            .Select(x => new
            {
                Name = x.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewData["Labels"] = new SelectList(labels, "Name", "Name");
    }
}
