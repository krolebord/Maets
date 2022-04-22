namespace Maets.Models.Dtos.Apps;

public class AppTableDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = null!;

    public DateTimeOffset? ReleaseDate { get; set; }
    
    public decimal Price { get; set; }
    
    public Guid PublisherId { get; set; }

    public string PublisherName { get; set; } = string.Empty;

    public ICollection<string> DeveloperNames { get; set; } = new List<string>();

    public int ScreenshotsCount { get; set; }
    
    public ICollection<string> Labels { get; set; } = new List<string>();
}
