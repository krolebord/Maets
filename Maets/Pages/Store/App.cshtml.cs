using AutoMapper;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Domain.Entities.Identity;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Maets.Models.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages;

public class AppPage : PageModel
{
    private readonly IMapper _mapper;
    private readonly MaetsDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    [BindProperty]
    public AppDetailedDto AppDto { get; set; } = null!;
    
    public AppPage(MaetsDbContext context, IMapper mapper, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _signInManager = signInManager;
    }
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        AppDto = await GetApp(id);
        
        return Page();
    }

    public async Task<IActionResult> OnPostAddToCollection(Guid id)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToPage();
        }

        if (!await AppIsInCollection(id))
        {
            _context.UserCollections.Add(new AppsUserCollection()
            {
                AppId = id,
                UserId = User.GetId()
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostRemoveFromCollection(Guid id)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToPage();
        }

        if (await AppIsInCollection(id))
        {
            var collectionEntry = await _context.UserCollections
                .FirstAsync(x => x.AppId == id && x.UserId == User.GetId());
            _context.UserCollections.Remove(collectionEntry);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    private Task<bool> AppIsInCollection(Guid appId)
    {
        return _context.UserCollections
            .AnyAsync(x => x.UserId == User.GetId() && x.AppId == appId);
    }
    
    private async Task<AppDetailedDto> GetApp(Guid id)
    {
        var app = await _context.Apps
            .Include(x => x.Publisher)
            .Include(x => x.Developers)
            .Include(x => x.MainImage)
            .Include(x => x.Screenshots)
            .Include(x => x.Labels)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (app is null)
        {
            throw new NotFoundException<App>();
        }

        var dto = _mapper.Map<AppDetailedDto>(app);

        if (_signInManager.IsSignedIn(User))
        {
            dto.IsInCollection = await AppIsInCollection(id);
        }

        return dto;
    }
}
