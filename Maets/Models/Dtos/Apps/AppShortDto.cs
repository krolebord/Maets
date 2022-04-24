using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Apps;

public class AppShortDto : EntityDto
{
    public string Title { get; set; } = string.Empty;

    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }
}
