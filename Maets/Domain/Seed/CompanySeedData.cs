using Maets.Domain.Entities;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed;

public class CompanySeedData : SeedData<Company>
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("6bd7ef6f-7497-4d20-bd18-729210af94a5");

    public override int Order => 10;

    protected override Task<bool> CheckIfInDatabaseAsync(DbSet<Company> set, Company entity)
    {
        return set.AnyAsync(x => x.Id == entity.Id);
    }

    protected override async Task<IEnumerable<Company>> GetEntities(DbContext context)
    {
        var company = new Company()
        {
            Id = DefaultCompanyId,
            Name = "Gaym Soft Works",
            Description = "Soft that works!!1!"
        };

        var dev = await context.Set<User>()
            .FirstAsync(x => x.Id.ToString() == DefaultUserData.Dev.Id);

        company.Employees.Add(new()
        {
            Id = Guid.NewGuid(),
            Company = company,
            User = dev
        });

        return new [] { company };
    }
}
