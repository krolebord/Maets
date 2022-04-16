using Microsoft.AspNetCore.Identity;

namespace Maets.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<string>
{
    public ICollection<ApplicationRole> Roles { get; set; } = new HashSet<ApplicationRole>();
}
