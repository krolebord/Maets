using Maets.Domain.Entities;

namespace Maets.Models.Dtos.MediaFiles;

public record AvatarFileDto(string? Key) : MediaFileDto(Key)
{
    public static AvatarFileDto From(MediaFile? file) => new(file?.Key);
}
