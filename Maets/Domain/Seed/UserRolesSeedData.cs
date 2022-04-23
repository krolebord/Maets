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
        var moderatorId = await usersSet.Select(x => x.Id).FirstAsync(x => x == DefaultUserData.Moderator.Id.ToString());

        var rolesSet = context.Set<ApplicationRole>();
        var adminRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Admin)).Id;
        var moderatorRoleId = (await rolesSet.FirstAsync(x => x.Name == RoleNames.Moderator)).Id;

        return new IdentityUserRole<string>[]
        {
            new() { RoleId = adminRoleId, UserId = adminId },
            new() { RoleId = moderatorRoleId, UserId = moderatorId },
        };
    }
}
