using Maets.Attributes;
using Maets.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Maets.Services.Identity.Implementations;

[Dependency(Lifetime = ServiceLifetime.Transient, Exposes = typeof(IUserValidator<ApplicationUser>))]
public class OptionalEmailUserValidator : UserValidator<ApplicationUser>
{
    public OptionalEmailUserValidator(IdentityErrorDescriber? errors = null)
        : base(errors) {}

    public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var result = await base.ValidateAsync(manager, user);

        if (result.Succeeded || !string.IsNullOrWhiteSpace(await manager.GetEmailAsync(user)))
            return result;

        var errors = result.Errors
            .Where(e => e.Code != "InvalidEmail")
            .ToArray();

        result = errors.Length > 0 ? IdentityResult.Failed(errors) : IdentityResult.Success;

        return result;
    }
}
