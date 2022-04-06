using Maets.Domain.Seed.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class IdentityUsersSeedData : SeedData<IdentityUser>
{
    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<IdentityUser> set, IdentityUser entity)
    {
        return set.AnyAsync(x => x.Id == entity.Id);
    }

    protected override Task<IEnumerable<IdentityUser>> GetEntities(DbContext context)
    {
        var passwordHasher = new PasswordHasher<IdentityUser>();

        var users = new IdentityUser[]
        {
            new()
            {
                Id = DefaultUserData.Admin.Id,
                UserName = DefaultUserData.Admin.UserName,
                Email = "admin@mail.com"
            },
            new()
            {
                Id = DefaultUserData.Dev.Id,
                UserName = DefaultUserData.Dev.UserName,
                Email = "dev@mail.com"
            },
            new()
            {
                Id = DefaultUserData.User.Id,
                UserName = DefaultUserData.User.UserName,
                Email = "user@mail.com"
            }
        };

        return Task.FromResult(users.Select(user =>
        {
            user.PasswordHash = passwordHasher.HashPassword(user, "pass12!");
            return user;
        }));
    }
}
