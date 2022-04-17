namespace Maets.Models.Dtos.Users;

public record UserForAdminDto(
    Guid Id,
    string UserName,
    string? Email,
    string? AvatarUrl,
    ICollection<string> Roles
);
