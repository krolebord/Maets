using Maets.Domain.Entities;
using Maets.Services.Files;

namespace Maets.Extensions;

public static class FileReadServiceExtensions
{
    public static string AvatarUrlOrDefault(this IFileReadService service, MediaFile? file)
    {
        return service.AvatarUrlOrDefault(file?.Key);
    }

    public static string ImageUrlOrDefault(this IFileReadService service, MediaFile? file)
    {
        return service.ImageUrlOrDefault(file?.Key);
    }

    public static string AvatarUrlOrDefault(this IFileReadService service, string? key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? MaetsAssets.AvatarPlaceholder
            : service.GetPublicUrl(key);
    }

    public static string ImageUrlOrDefault(this IFileReadService service, string? key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? MaetsAssets.ImagePlaceholder
            : service.GetPublicUrl(key);
    }
}
