namespace Maets.Domain.Seed.Common;

public record DefaultUserData(
    string Id,
    string UserName
)
{
    public static readonly DefaultUserData Admin =
        new ("0172a401-f197-4315-a760-a16c16d7b882", "Admin");

    public static readonly DefaultUserData Dev =
        new ("3d561bbc-2a91-4764-b176-825625bfd6d7", "Dev");

    public static readonly DefaultUserData User =
        new ("4313a59d-1828-4460-b6a6-6e27a19ad130", "User");
}
