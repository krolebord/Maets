using AutoMapper;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.MediaFiles;
using Maets.Services.Files;

namespace Maets.Services.Converters;

public class AvatarFileToUrlConverter : ITypeConverter<AvatarFileDto?, string>
{
    private readonly IFileReadService _fileReadService;

    public AvatarFileToUrlConverter(IFileReadService fileReadService)
    {
        _fileReadService = fileReadService;
    }

    public string Convert(AvatarFileDto? sourceMember, string destination, ResolutionContext context)
    {
        return _fileReadService.AvatarUrlOrDefault(sourceMember?.Key);
    }
}
