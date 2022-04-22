using Maets.Models.Dtos.Companies;

namespace Maets.Models.Dtos.Apps;

public class AppDetailedDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }

    public CompanyShortDto Publisher { get; set; } = null!;

    public ICollection<CompanyShortDto> Developers { get; set; } = new List<CompanyShortDto>();

    public string MainImageUrl { get; set; } = string.Empty;

    public ICollection<string> ScreenshotUrls { get; set; } = new List<string>();

    public ICollection<string> Labels { get; set; } = new List<string>();
}
