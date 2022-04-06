using Maets.Domain.Constants;
using Maets.Domain.Seed.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class RolesSeedData : SeedData<IdentityRole>
{
    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<IdentityRole> set, IdentityRole entity)
    {
        return set.AnyAsync(x => x.Name == entity.Name);
    }

    protected override Task<IEnumerable<IdentityRole>> GetEntities(DbContext context)
    {
        return Task.FromResult(new IdentityRole[]
        {
            new(RoleNames.Admin),
            new(RoleNames.Developer)
        }.AsEnumerable());
    }
}
