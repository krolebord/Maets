namespace Maets.Models.Dtos.Companies;

public record CompanyReadDto(
    Guid Id,
    string Name,
    string Description,
    string PhotoUrl
);
