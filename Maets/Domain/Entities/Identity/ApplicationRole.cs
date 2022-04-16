using Microsoft.AspNetCore.Identity;

namespace Maets.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<string>
{
    public ApplicationRole(string name) : base(name) {}

    public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
}
