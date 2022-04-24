using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Users;

public class UserReadDto : EntityDto
{
    public string UserName { get; init; } = null!;

    public string? AvatarUrl { get; init; }

    private UserReadDto() {}

    public UserReadDto(
        Guid id,
        string userName,
        string? avatarUrl
    )
    {
        Id = id;
        UserName = userName;
        AvatarUrl = avatarUrl;
    }
}
