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
    
    public ICollection<AppHomeDto> UpcomingReleases { get; set; } = new List<AppHomeDto>();

    public HomePage(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task OnGetAsync()
    {
        var topAppsQuery = _context.Apps
            .OrderBy(x => x.Title) // TODO Order by top
            .Take(5);
        TopApps = await GetAppsFromQuery(topAppsQuery);

        var newReleasesQuery = _context.Apps
            .Where(x => x.ReleaseDate != null)
            .OrderByDescending(x => x.ReleaseDate!.Value)
            .Take(5);
        NewReleases = await GetAppsFromQuery(newReleasesQuery);
        
        var upcomingReleasesQuery = _context.Apps
            .Where(x => x.ReleaseDate == null)
            .OrderBy(x => x.Title) // TODO Order by collections
            .Take(5);
        UpcomingReleases = await GetAppsFromQuery(upcomingReleasesQuery);
    }

    private async Task<ICollection<AppHomeDto>> GetAppsFromQuery(IQueryable<App> queryable)
    {
        var apps = await queryable.ToListAsync();
        return _mapper.Map<List<AppHomeDto>>(apps);
    }
}
