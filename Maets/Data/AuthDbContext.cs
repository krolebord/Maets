using Maets.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Maets.Data;

public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<ApplicationUser>(builder => {
            builder
                .HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<IdentityUserRole<string>>(
                    right => right.HasOne<ApplicationRole>()
                        .WithMany()
                        .HasForeignKey(x => x.RoleId)
                        .IsRequired(),
                    left => left.HasOne<ApplicationUser>()
                        .WithMany()
                        .HasForeignKey(x => x.UserId)
                        .IsRequired()
                );
        });
    }
}
