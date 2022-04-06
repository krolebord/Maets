using Maets.Domain.Entities;
using Maets.Domain.Seed.Common;
using Maets.Models;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class LabelsSeedData : SeedData<Label>
{
    public static readonly string[] DefaultLabelNames = new[]
    {
        "WebApp",
        "Indie",
        "Shooter",
        "RPG",
        "Adventure",
        "Multiplayer",
        "MOBA",
        "MMO",
        "Racing",
        "Simulator",
    };

    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<Label> set, Label entity)
    {
        return set.AnyAsync(x => x.Name == entity.Name);
    }

    protected override Task<IEnumerable<Label>> GetEntities(DbContext context)
    {
        return Task.FromResult(DefaultLabelNames
            .Select(name => new Label()
            {
                Id = Guid.NewGuid(),
                Name = name
            }));
    }
}
