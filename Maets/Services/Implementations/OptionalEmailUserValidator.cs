using Maets.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Maets.Services.Implementations;

[Dependency(Lifetime = ServiceLifetime.Transient, Exposes = typeof(IUserValidator<IdentityUser>))]
public class OptionalEmailUserValidator : UserValidator<IdentityUser>
{
    public OptionalEmailUserValidator(IdentityErrorDescriber? errors = null)
        : base(errors) {}

    public override async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
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
