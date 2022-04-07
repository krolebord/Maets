using Maets.Domain.Entities;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class MaetsUsersData : SeedData<User>
{
    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<User> set, User entity)
    {
        return set.AnyAsync(x => x.Id == entity.Id);
    }

    protected override Task<IEnumerable<User>> GetEntities(DbContext context)
    {
        var users = new User[]
        {
            new()
            {
                Id = Guid.Parse(DefaultUserData.Admin.Id),
                UserName = DefaultUserData.Admin.UserName
            },
            new()
            {
                Id = Guid.Parse(DefaultUserData.Dev.Id),
                UserName = DefaultUserData.Dev.UserName
            },
            new()
            {
                Id = Guid.Parse(DefaultUserData.User.Id),
                UserName = DefaultUserData.User.UserName
            }
        };

        return Task.FromResult(users.AsEnumerable());
    }
}
