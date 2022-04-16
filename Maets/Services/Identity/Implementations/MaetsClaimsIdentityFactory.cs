using System.Security.Claims;
using Maets.Attributes;
using Maets.Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Maets.Services.Identity.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IUserClaimsPrincipalFactory<IdentityUser>))]
public class MaetsClaimsIdentityFactory : UserClaimsPrincipalFactory<IdentityUser>
{
    private readonly IUsersService _usersService;

    public MaetsClaimsIdentityFactory(
        IOptions<IdentityOptions> optionsAccessor,
        UserManager<IdentityUser> userManager,
        IUsersService usersService
    ) : base(userManager, optionsAccessor)
    {
        _usersService = usersService;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
    {
        var principal = await base.CreateAsync(user);

        var avatarUrl = await _usersService.GetAvatarUrl(Guid.Parse(user.Id));

        var claimsIdentity = (ClaimsIdentity) principal.Identity!;
        claimsIdentity.AddClaim(new(MaetsClaims.AvatarUrl, avatarUrl));

        return principal;
    }
}
