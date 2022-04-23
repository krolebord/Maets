using AutoMapper;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages;

public class SearchPage : PageModel
{
    private readonly MaetsDbContext _context;

    private readonly IMapper _mapper;

    public bool ValidQuery { get; set; } = false;

    public ICollection<AppHomeDto> Results { get; set; } = new List<AppHomeDto>();
    
    public SelectList CompaniesList { get; set; } = null!;
    public SelectList LabelsList { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Company { get; set; }

    [BindProperty(SupportsGet = true)]
    public IEnumerable<string>? Labels { get; set; } = null;
    
    public SearchPage(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task OnGetAsync()
    {
        await LoadSelectLists();
        
        var query = _context.Apps
            .WhereIf(x => x.Title.Contains(Query!), Query is not null)
            .WhereIf(x => x.Publisher!.Name == Company || x.Developers.Any(company => company.Name == Company), Company is not null)
            .WhereIf(x => x.Labels.Any(label => Labels!.Contains(label.Name)), Labels?.Any() == true);

        if (query is DbSet<App>)
        {
            ValidQuery = false;
            return;
        }

        ValidQuery = true;
        Results = _mapper.Map<List<AppHomeDto>>(await query.ToListAsync());
    }

    private async Task LoadSelectLists()
    {
        var companies = await _context.Companies
            .Select(x => x.Name)
            .ToListAsync();

        var selectedCompany = Company is null ? null
            : companies.FirstOrDefault(x => x.Equals(Company, StringComparison.InvariantCultureIgnoreCase));
        
        CompaniesList = new SelectList(companies, selectedCompany);
        

        var labels = await _context.Labels
            .Select(x => x.Name)
            .ToListAsync();

        var selectedValues = Labels?.Any() == false
            ? Enumerable.Empty<string>()
            : labels.Where(x => Labels!.Any(label => label.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
        LabelsList = new SelectList(labels, selectedValues);
    }
}
