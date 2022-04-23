using System.Security.Claims;
using Maets.Domain.Constants;
using Maets.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Maets.Extensions;

public static class UserExtensions
{
    public static bool IsAdmin(this SignInManager<ApplicationUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Admin);
    }
    
    public static bool IsModerator(this SignInManager<ApplicationUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Moderator);
    }
    
    public static bool IsAdminOrModerator(this SignInManager<ApplicationUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && (user.IsInRole(RoleNames.Moderator) || user.IsInRole(RoleNames.Admin));
    }
}
