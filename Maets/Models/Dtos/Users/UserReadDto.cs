namespace Maets.Models.Dtos.Users;

public record UserReadDto
{
    public Guid Id { get; init; } = Guid.Empty;

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
