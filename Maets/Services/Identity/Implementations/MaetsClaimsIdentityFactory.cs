using System.Security.Claims;
using Maets.Attributes;
using Maets.Domain.Constants;
using Maets.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Maets.Services.Identity.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IUserClaimsPrincipalFactory<ApplicationUser>))]
public class MaetsClaimsIdentityFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly IUsersService _usersService;

    public MaetsClaimsIdentityFactory(
        IOptions<IdentityOptions> optionsAccessor,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUsersService usersService
    ) : base(userManager, roleManager, optionsAccessor)
    {
        _usersService = usersService;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);

        var avatarUrl = await _usersService.GetAvatarUrl(Guid.Parse(user.Id));

        var claimsIdentity = (ClaimsIdentity) principal.Identity!;
        claimsIdentity.AddClaim(new(MaetsClaims.AvatarUrl, avatarUrl));

        return principal;
    }
}
