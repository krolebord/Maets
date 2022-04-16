namespace Maets.Models.Dtos.User;

public record UserForAdminDto(
    Guid Id,
    string UserName,
    string? Email,
    string? AvatarUrl,
    ICollection<string> Roles
);
