using System.Security.Claims;
using Maets.Domain.Constants;

namespace Maets.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetAvatarUrl(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(MaetsClaims.AvatarUrl);
    }

    public static Guid GetId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
