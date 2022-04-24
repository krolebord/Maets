using Maets.Domain.Entities.Identity;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Maets.Models.Dtos.Reviews;
using Maets.Services.Apps;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Maets.Pages;

public class AppPage : PageModel
{
    private readonly IAppsService _appsService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    
    public AppDetailedDto AppDto { get; set; } = null!;

    public ReviewReadDto? UserReview { get; set; } = null;

    public AppPage(SignInManager<ApplicationUser> signInManager, IAppsService appsService)
    {
        _signInManager = signInManager;
        _appsService = appsService;
    }
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        AppDto = await _appsService.GetDetailed(id, User.FindId());

        if (_signInManager.IsSignedIn(User))
        {
            UserReview = await _appsService.FindUserReview(id, User.GetId());
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAddToCollection(Guid id)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToPage();
        }

        await _appsService.EnsureInCollection(id, User.GetId());

        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPostRemoveFromCollection(Guid id)
    {
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToPage();
        }

        await _appsService.EnsureRemovedFromCollection(id, User.GetId()); 

        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetAppReviews(Guid id)
    {
        var reviews = await _appsService.GetReviews(id);

        return new PartialViewResult
        {
            ViewName = "Components/_ReviewPartial",
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = reviews
            }
        };
    }
}
