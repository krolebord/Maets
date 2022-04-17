namespace Maets.Models.Dtos.Company;

public record CompanyReadDto(
    Guid Id,
    string Name,
    string Description,
    string PhotoUrl
);
