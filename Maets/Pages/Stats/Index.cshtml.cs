using Maets.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages.Stats;

public class StatsPage : PageModel
{
    private readonly MaetsDbContext _context;

    public ICollection<LabelInfo> LabelsStats { get; set; } = new List<LabelInfo>();

    public StatsPage(MaetsDbContext context)
    {
        _context = context;
    }

    public class LabelInfo
    {
        public string LabelName { get; set; } = string.Empty;
        public int AppsCount { get; set; }
    }
    
    public async Task OnGet()
    {
        LabelsStats = await _context.Labels
            .Select(x => new LabelInfo
            {
                LabelName = x.Name,
                AppsCount = x.AppsLabels.Count
            })
            .ToListAsync();
    }
}
