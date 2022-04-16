using Maets.Domain.Constants;
using Maets.Domain.Entities.Identity;
using Maets.Domain.Seed.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class UserRolesSeedData : SeedData<IdentityUserRole<string>>
{
    public override int Order => 10;

    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<IdentityUserRole<string>> set, IdentityUserRole<string> entity)
    {
        return set.AnyAsync(x => x.RoleId == entity.RoleId && x.UserId == entity.UserId);
    }

    protected override async Task<IEnumerable<IdentityUserRole<string>>> GetEntities(DbContext context)
    {
        var usersSet = context.Set<ApplicationUser>();
        var adminId = await usersSet.Select(x => x.Id).FirstAsync(x => x == DefaultUserData.Admin.Id.ToString());
        var devId = await usersSet.Select(x => x.Id).FirstAsync(x => x == DefaultUserData.Dev.Id.ToString());

        var rolesSet = context.Set<ApplicationRole>();
        var adminRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Admin)).Id;
        var devRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Developer)).Id;

        return new IdentityUserRole<string>[]
        {
            new() { RoleId = adminRoleId, UserId = adminId },
            new() { RoleId = devRoleId, UserId = devId }
        };
    }
}
