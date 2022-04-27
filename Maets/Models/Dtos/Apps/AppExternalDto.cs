namespace Maets.Models.Dtos.Apps;

public class AppExternalDto
{
    public string Title { get; set; } = null!;

    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }

    public string PublisherName { get; set; } = string.Empty;

    public IEnumerable<string> DeveloperNames { get; set; } = new List<string>();

    public IEnumerable<string> Labels { get; set; } = new List<string>();
}
