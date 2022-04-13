using System.Security.Claims;
using Maets.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Maets.Extensions;

public static class UserExtensions
{
    public static bool IsAdmin(this SignInManager<IdentityUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Admin);
    }

    public static bool IsDev(this SignInManager<IdentityUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Developer);
    }
}
