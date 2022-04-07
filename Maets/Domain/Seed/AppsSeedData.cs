using Maets.Domain.Entities;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Maets.Domain.Seed;

public class AppsSeedData : SeedData<App>
{
    public const string DefaultAppTitle = "Maets";

    public override int Order => 100;

    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<App> set, App entity)
    {
        return set.AnyAsync(x => x.Title == entity.Title);
    }

    protected override async Task<IEnumerable<App>> GetEntities(DbContext context)
    {
        var defaultCompany = await context.Set<Company>()
            .FirstAsync(x => x.Id == CompanySeedData.DefaultCompanyId);

        var labelNames = LabelsSeedData.DefaultLabelNames.Take(2).ToHashSet();
        var defaultLabels = await context.Set<Label>()
            .Where(label => labelNames.Contains(label.Name))
            .ToListAsync();

        var app = new App
        {
            Id = Guid.NewGuid(),
            Title = DefaultAppTitle,
            Description = "Not a steam clone",
            Price = 0,
            ReleaseDate = DateTimeOffset.Now,
            Publisher = defaultCompany
        };

        app.Developers.Add(defaultCompany);

        app.Labels.AddRange(defaultLabels);

        return new[] { app };
    }
}
