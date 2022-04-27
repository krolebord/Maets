using AutoMapper;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Models.Dtos.Apps;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages;

public class HomePage : PageModel
{
    private readonly MaetsDbContext _context;

    private readonly IMapper _mapper;
    
    public ICollection<AppHomeDto> TopApps { get; set; } = new List<AppHomeDto>();
    
    public ICollection<AppHomeDto> NewReleases { get; set; } = new List<AppHomeDto>();
    
    public ICollection<AppHomeDto> TopSellers { get; set; } = new List<AppHomeDto>();

    public HomePage(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task OnGetAsync()
    {
        var topAppsQuery = _context.Apps
            .OrderByDescending(x => x.Reviews.Average(r => r.Score))
            .Take(5);
        TopApps = await GetAppsFromQuery(topAppsQuery);

        var newReleasesQuery = _context.Apps
            .Where(x => x.ReleaseDate != null)
            .OrderByDescending(x => x.ReleaseDate!.Value)
            .Take(5);
        NewReleases = await GetAppsFromQuery(newReleasesQuery);
        
        var topSellersQuery = _context.Apps
            .OrderByDescending(x => x.InUserCollections.Count)
            .Take(5);
        TopSellers = await GetAppsFromQuery(topSellersQuery);
    }

    private async Task<ICollection<AppHomeDto>> GetAppsFromQuery(IQueryable<App> queryable)
    {
        var apps = await queryable
            .Include(x => x.Publisher)
            .Include(x => x.MainImage)
            .Include(x => x.Developers)
            .Include(x => x.Labels)
            .ToListAsync();
        return _mapper.Map<List<AppHomeDto>>(apps);
    }
}
