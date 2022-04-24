using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Companies;

public class CompanyReadDto : EntityDto
{
    public string Name { get; init; }

    public string Description { get; init; }

    public string PhotoUrl { get; init; }

    public int PublishedAppsCount { get; init; }

    public int DevelopedAppsCount { get; init; }

    public int EmployeesCount { get; init; }

    private CompanyReadDto()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Description = string.Empty;
        PhotoUrl = string.Empty;
    }

    public CompanyReadDto(Guid id,
        string name,
        string description,
        string photoUrl)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.PhotoUrl = photoUrl;
    }
}
