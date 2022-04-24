using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Companies;

public class CompanyShortDto : EntityDto
{
    public string Name { get; init; } = string.Empty;
}
