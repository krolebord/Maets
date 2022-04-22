using AutoMapper;
using Maets.Data;
using Maets.Domain.Entities.Identity;
using Maets.Models.Dtos.Apps;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Maets.Pages;

public class App : PageModel
{
    private readonly IMapper _mapper;
    private readonly MaetsDbContext _context;

    [BindProperty]
    public AppDetailedDto AppDto { get; set; } = null!;
    
    public App(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IActionResult> OnGetAsync(Guid id)
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
            return RedirectToPage("NotFound");
        }

        AppDto = _mapper.Map<AppDetailedDto>(app);
        
        return Page();
    }
}
