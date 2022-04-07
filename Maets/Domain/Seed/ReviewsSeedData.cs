using Maets.Domain.Entities;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class ReviewsSeedData : SeedData<Review>
{
    public static readonly Guid DefaultReviewId = new("a85c077b-629b-451c-ade0-1d2d99d592cc");

    public override int Order => 200;

    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<Review> set, Review entity)
    {
        return set.AnyAsync(x => x.Id == entity.Id);
    }

    protected override async Task<IEnumerable<Review>> GetEntities(DbContext context)
    {
        var author = await context.Set<User>()
            .FirstAsync(x => x.Id.ToString() == DefaultUserData.User.Id);

        var app = await context.Set<App>()
            .FirstAsync(x => x.Title == AppsSeedData.DefaultAppTitle);

        var review = new Review
        {
            Id = DefaultReviewId,
            Title = "Maets honest review",
            Description = "This is definitely not a steam clone!",
            Score = 99,
            App = app,
            Author = author
        };

        return new[] { review };
    }
}
