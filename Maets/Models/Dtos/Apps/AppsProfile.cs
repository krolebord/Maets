using AutoMapper;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.MediaFiles;
using Newtonsoft.Json;

namespace Maets.Models.Dtos.Apps;

public class AppsProfile : Profile
{
    public AppsProfile()
    {
        CreateMap<App, AppTableDto>()
            .ForMember(x => x.DeveloperNames, opt => opt.MapFrom(x => x.Developers.Select(dev => dev.Name)))
            .ForMember(x => x.PublisherName, opt => opt.MapFrom(x => x.Publisher == null ? string.Empty : x.Publisher.Name));

        CreateMap<App, AppExternalDto>()
            .ForMember(x => x.ReleaseDate, opt => opt.MapFrom(x => x.ReleaseDate == null ? string.Empty : x.ReleaseDate.Value.ToString("O")))
            .ForMember(x => x.DeveloperNames, opt => opt.MapFrom(x => string.Join(",", x.Developers.Select(dev => dev.Name))))
            .ForMember(x => x.PublisherName, opt => opt.MapFrom(x => x.Publisher == null ? string.Empty : x.Publisher.Name))
            .ForMember(x => x.Labels, opt => opt.MapFrom(x => string.Join(",", x.Labels.Select(dev => dev.Name))));

        CreateMap<App, AppDetailedDto>()
            .MapImageUrl(x => x.MainImageUrl, app => app.MainImage)
            .ForMember(x => x.ScreenshotUrls, opt => opt.MapFrom(x => x.Screenshots.Select(screenshot => new ImageFileDto(screenshot.Key))));

        CreateMap<App, AppHomeDto>()
            .MapImageUrl(x => x.MainImageUrl, app => app.MainImage);

        CreateMap<App, AppShortDto>();
        
        CreateMap<App, AppEditDto>()
            .ForMember(x => x.DeveloperIds, opt => opt.MapFrom(app => app.Developers.Select(dev => dev.Id)));
        CreateMap<AppEditDto, App>()
            .ForMember(x => x.Labels, opt => opt.Ignore())
            .ForMember(x => x.MainImageId, opt => opt.Ignore())
            .ForMember(x => x.MainImage, opt => opt.Ignore())
            .ForMember(x => x.Screenshots, opt => opt.Ignore());
    }
}
