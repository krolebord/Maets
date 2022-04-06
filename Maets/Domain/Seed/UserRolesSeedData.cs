using Maets.Domain.Constants;
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
        var usersSet = context.Set<IdentityUser>();
        var adminId = await usersSet.Select(x => x.Id).FirstAsync(x => x == DefaultUserData.Admin.Id);
        var devId = await usersSet.Select(x => x.Id).FirstAsync(x => x == DefaultUserData.Dev.Id);

        var rolesSet = context.Set<IdentityRole>();
        var adminRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Admin)).Id;
        var devRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Developer)).Id;

        return new IdentityUserRole<string>[]
        {
            new() { RoleId = adminRoleId, UserId = adminId },
            new() { RoleId = devRoleId, UserId = devId }
        };
    }
}
