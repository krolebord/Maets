using System.ComponentModel.DataAnnotations;

namespace Maets.Models.Dtos.Companies;

public class CompanyWriteDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;

    public IFormFile? Photo { get; set; }

    public IEnumerable<Guid> EmployeeIds { get; set; } = new HashSet<Guid>();
}
