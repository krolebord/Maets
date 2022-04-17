using System.ComponentModel.DataAnnotations;

namespace Maets.Models.Dtos.Companies;

public class CompanyWriteDto
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    public IFormFile? Photo { get; set; }
}
