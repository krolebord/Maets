using AutoMapper;
using Maets.Attributes;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Domain.Entities.Identity;
using Maets.Extensions;
using Maets.Models.Dtos.Apps;
using Maets.Models.Dtos.Reviews;
using Maets.Models.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Services.Apps;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IAppsService))]
public class AppService : IAppsService
{
    private readonly MaetsDbContext _context;
    private readonly IMapper _mapper;

    public AppService(
        IHttpContextAccessor httpContextAccessor,
        MaetsDbContext context,
        IMapper mapper,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<AppDetailedDto> GetDetailed(Guid appId, Guid? userId)
    {
        var app = await _context.Apps
            .Include(x => x.Publisher)
            .Include(x => x.Developers)
            .Include(x => x.MainImage)
            .Include(x => x.Screenshots)
            .Include(x => x.Labels)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == appId);

        if (app is null)
        {
            throw new NotFoundException<App>();
        }

        var dto = _mapper.Map<AppDetailedDto>(app);
        
        if (userId is not null)
        {
            dto.IsInCollection = await IsAppInCollection(appId, userId.Value);
        }

        return dto;
    }
    
    private IEnumerable<DateTime> GetMonths(DateTime startDate, DateTime endDate)
    {
        startDate = new DateTime(startDate.Year, startDate.Month + 1, 1);
        endDate = new DateTime(endDate.Year, endDate.Month + 1, 1);

        while (startDate < endDate)
        {
            yield return startDate;
            startDate = startDate.AddMonths(1);
        }
    }
    
    public async Task<AppReviewsDto> GetReviews(Guid appId)
    {
        var resultDto = new AppReviewsDto();

        resultDto.AverageScore = (int)await _context.Reviews
            .Where(x => x.AppId == appId)
            .AverageAsync(x => x.Score);

        var reviews = await _context.Reviews
            .Include(x => x.App)
            .Include(x => x.Author)
            .Where(x => x.AppId == appId)
            .ToListAsync();
        resultDto.Reviews = _mapper.Map<List<ReviewReadDto>>(reviews);

        var currentDate = DateTime.Now;
        var statisticsStart = currentDate.AddMonths(-6);

        var months = GetMonths(statisticsStart, currentDate);

        var reviewsData = await _context.Reviews
            .Where(x => x.CreationDate >= statisticsStart)
            .Select(x => new
            {
                Year = x.CreationDate.Year,
                Month = x.CreationDate.Month,
                Score = x.Score
            })
            .GroupBy(x => new
            {
                Year = x.Year,
                Month = x.Month,
            })
            .Select(x => new AppReviewsDto.AppReviewScoresGroup
            {
                Year = x.Key.Year,
                Month = x.Key.Month,
                AverageScore = x.Average(s => s.Score)
            })
            .ToListAsync();

        var query =
            from m in months
            join s in reviewsData on new { m.Year, m.Month } equals new { s.Year, s.Month } into gj
            from s in gj.DefaultIfEmpty()
            select s ?? new AppReviewsDto.AppReviewScoresGroup
            {
                Year = m.Year,
                Month = m.Month,
                AverageScore = 0
            };

        resultDto.ScoresData = query.ToList();

        return resultDto;
    }
    
    public async Task<ReviewReadDto?> FindUserReview(Guid appId, Guid userId)
    {
        var review = await _context.Reviews
            .Include(x => x.App)
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.AppId == appId && x.AuthorId == userId);
        return _mapper.Map<ReviewReadDto>(review);
    }

    public Task<bool> IsAppInCollection(Guid appId, Guid userid)
    {
        return _context.UserCollections
            .AnyAsync(x => x.UserId == userid && x.AppId == appId);
    }
    
    public async Task EnsureInCollection(Guid appId, Guid userId)
    {
        if (await IsAppInCollection(appId, userId))
        {
            return;
        }
        
        _context.UserCollections.Add(new AppsUserCollection()
        {
            AppId = appId,
            UserId = userId
        });
        await _context.SaveChangesAsync();
    }
    
    public async Task EnsureRemovedFromCollection(Guid appId, Guid userId)
    {
        if (!await IsAppInCollection(appId, userId))
        {
            return;
        }
        
        var collectionEntry = await _context.UserCollections
            .FirstAsync(x => x.AppId == appId && x.UserId == userId);
        _context.UserCollections.Remove(collectionEntry);
        await _context.SaveChangesAsync();
    }
}
