using AutoMapper;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.MediaFiles;
using Maets.Services.Files;

namespace Maets.Services.Converters;

public class ImageFileToUrlConverter : ITypeConverter<ImageFileDto?, string>
{
    private readonly IFileReadService _fileReadService;

    public ImageFileToUrlConverter(IFileReadService fileReadService)
    {
        _fileReadService = fileReadService;
    }

    public string Convert(ImageFileDto? sourceMember, string destination, ResolutionContext context)
    {
        return _fileReadService.ImageUrlOrDefault(sourceMember?.Key);
    }
}
