using AutoMapper;
using Maets.Domain.Entities;
using Maets.Models.Dtos.MediaFiles;
using Maets.Services.Files;

namespace Maets.Services.Converters;

public class MediaFileToUrlConverter : ITypeConverter<MediaFile?, string?>
{
    private readonly IFileReadService _fileReadService;

    public MediaFileToUrlConverter(IFileReadService fileReadService)
    {
        _fileReadService = fileReadService;
    }

    public string? Convert(MediaFile? source, string? destination, ResolutionContext context)
    {
        return source?.Key is null ? null : _fileReadService.GetPublicUrl(source.Key);
    }
}

public class MediaFileDtoToUrlConverter : ITypeConverter<MediaFileDto?, string?>
{
    private readonly IFileReadService _fileReadService;

    public MediaFileDtoToUrlConverter(IFileReadService fileReadService)
    {
        _fileReadService = fileReadService;
    }

    public string? Convert(MediaFileDto? source, string? destination, ResolutionContext context)
    {
        return source?.Key is null ? null : _fileReadService.GetPublicUrl(source.Key);
    }
}
