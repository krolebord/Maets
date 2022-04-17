using Maets.Domain.Entities;

namespace Maets.Models.Dtos.MediaFiles;

public record ImageFileDto(string? Key) : MediaFileDto(Key)
{
    public static ImageFileDto From(MediaFile? file) => new(file?.Key);
}
