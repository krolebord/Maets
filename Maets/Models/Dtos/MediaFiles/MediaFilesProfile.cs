using AutoMapper;
using Maets.Domain.Entities;
using Maets.Services.Converters;

namespace Maets.Models.Dtos.MediaFiles;

public class MediaFilesProfile : Profile
{
    public MediaFilesProfile()
    {
        CreateMap<MediaFile?, string?>().ConvertUsing<MediaFileToUrlConverter>();
        CreateMap<MediaFileDto?, string?>().ConvertUsing<MediaFileDtoToUrlConverter>();
        CreateMap<ImageFileDto?, string>().ConvertUsing<ImageFileToUrlConverter>();
        CreateMap<AvatarFileDto?, string>().ConvertUsing<AvatarFileToUrlConverter>();
    }
}
