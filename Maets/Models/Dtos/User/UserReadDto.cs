namespace Maets.Models.Dtos.User;

public record UserReadDto(
    Guid Id,
    string UserName,
    string? Email,
    string? AvatarUrl
);
