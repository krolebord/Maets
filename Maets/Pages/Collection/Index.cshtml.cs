using AutoMapper;
using Maets.Data;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages.Collection;

public class CollectionPage : PageModel
{
    private readonly MaetsDbContext _context;
    private readonly IMapper _mapper;
    
    public ICollection<AppHomeDto> Collection { get; set; } = new List<AppHomeDto>();

    public CollectionPage(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task OnGet()
    {
        var userId = User.GetId();
        var apps = await _context.Apps
            .Include(x => x.Publisher)
            .Include(x => x.MainImage)
            .Include(x => x.Developers)
            .Include(x => x.Labels)
            .Where(x => x.InUserCollections.Any(user => user.Id == userId))
            .ToListAsync();

        Collection = _mapper.Map<List<AppHomeDto>>(apps);
    }
}
