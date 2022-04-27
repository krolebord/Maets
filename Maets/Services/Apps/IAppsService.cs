using Maets.Domain.Entities;
using Maets.Models.Dtos.Apps;
using Maets.Models.Dtos.Reviews;

namespace Maets.Services.Apps;

public interface IAppsService
{
    Task<AppDetailedDto> GetDetailed(Guid id, Guid? userId);
    
    Task<AppReviewsDto> GetReviews(Guid appId);

    Task<ReviewReadDto?> FindUserReview(Guid appId, Guid userId);

    Task<bool> IsAppInCollection(Guid appId, Guid userid);

    Task EnsureInCollection(Guid appId, Guid userId);
    
    Task EnsureRemovedFromCollection(Guid appId, Guid userId);

    Task CreateApp(AppCreateDto createDto);

    Task CreateApp(AppExternalDto createDto);

    string BuildAppScreenshotKey(Guid appId);
}
