using System.ComponentModel.DataAnnotations;

namespace Maets.Models.Dtos.Apps;

public class AppCreateDto
{
    [Required]
    [MinLength(4)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(10)]
    public string Description { get; set; } = string.Empty;

    public DateTimeOffset? ReleaseDate { get; set; }

    public decimal Price { get; set; }

    [Required]
    public Guid? PublisherId { get; set; }

    public IEnumerable<Guid> DeveloperIds { get; set; } = new HashSet<Guid>();

    public IEnumerable<string> Labels { get; set; } = new HashSet<string>();
    
    [Required]
    public IFormFile MainImage { get; set; } = null!;

    public IEnumerable<IFormFile>? Screenshots { get; set; } = Array.Empty<IFormFile>();
}
