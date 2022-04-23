using Maets.Domain.Constants;
using Maets.Domain.Entities.Identity;
using Maets.Domain.Seed.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class RolesSeedData : SeedData<ApplicationRole>
{
    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<ApplicationRole> set, ApplicationRole entity)
    {
        return set.AnyAsync(x => x.Name == entity.Name);
    }

    protected override Task<IEnumerable<ApplicationRole>> GetEntities(DbContext context)
    {
        return Task.FromResult(new ApplicationRole[]
        {
            new(RoleNames.Admin)
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = RoleNames.Admin.ToUpper()
            },
            new(RoleNames.Moderator)
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = RoleNames.Moderator.ToUpper()
            }
        }.AsEnumerable());
    }
}
