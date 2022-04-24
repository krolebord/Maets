using Maets.Models.Dtos.Companies;
using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Apps;

public class AppHomeDto : EntityDto
{
    public string Title { get; set; } = string.Empty;

    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }

    public CompanyShortDto Publisher { get; set; } = null!;

    public ICollection<CompanyShortDto> Developers { get; set; } = new List<CompanyShortDto>();

    public string MainImageUrl { get; set; } = string.Empty;

    public ICollection<string> Labels { get; set; } = new List<string>();
}
