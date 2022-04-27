namespace Maets.Models.Dtos.Apps;

public class AppExternalDto
{
    public string Title { get; set; } = null!;

    public string? ReleaseDate { get; set; }
    
    public double Price { get; set; }

    public string PublisherName { get; set; } = string.Empty;

    public string DeveloperNames { get; set; } = string.Empty;

    public string Labels { get; set; } = string.Empty;
}
