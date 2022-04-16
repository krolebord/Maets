using Maets.Domain.Entities.Identity;
using Maets.Domain.Seed.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class IdentityUsersSeedData : ISeedData<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public int Order => 0;

    public IdentityUsersSeedData(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task ApplyAsync(DbContext context)
    {
        var users = new ApplicationUser[]
        {
            new()
            {
                Id = DefaultUserData.Admin.Id.ToString(),
                UserName = DefaultUserData.Admin.UserName,
                Email = "admin@mail.com",
                EmailConfirmed = true
            },
            new()
            {
                Id = DefaultUserData.Dev.Id.ToString(),
                UserName = DefaultUserData.Dev.UserName,
                Email = "dev@mail.com",
                EmailConfirmed = true
            },
            new()
            {
                Id = DefaultUserData.User.Id.ToString(),
                UserName = DefaultUserData.User.UserName,
                Email = "user@mail.com",
                EmailConfirmed = true
            }
        };

        foreach (var user in users)
        {
            if (await _userManager.FindByIdAsync(user.Id) is not null)
            {
                continue;
            }

            await _userManager.CreateAsync(user, "Password12!");
        }
    }
}
