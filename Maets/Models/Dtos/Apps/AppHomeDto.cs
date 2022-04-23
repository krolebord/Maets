using Maets.Models.Dtos.Companies;

namespace Maets.Models.Dtos.Apps;

public class AppHomeDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }

    public string Publisher { get; set; } = null!;

    public ICollection<CompanyShortDto> Developers { get; set; } = new List<CompanyShortDto>();

    public string MainImageUrl { get; set; } = string.Empty;

    public ICollection<CompanyShortDto> Labels { get; set; } = new List<CompanyShortDto>();
}
