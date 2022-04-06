using Maets.Data;
using Maets.Domain.Seed;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;

namespace Maets.Extensions;

public static class SeedingExtensions
{
    private record struct SeedDataInfo(Type EntityType, ISeedData SeedData);

    public static async Task SeedAndMigrateAsync(this WebApplication app)
    {
        var seedDataInfos = typeof(ISeedData<>).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .Select(t => new {
                EntityType = t.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeedData<>))
                    ?.GetGenericArguments().FirstOrDefault(),
                SeedDataType = t
            })
            .Where(x => x.EntityType is not null)
            .Select(x => new SeedDataInfo(x.EntityType!,(ISeedData)Activator.CreateInstance(x.SeedDataType)!))
            .OrderBy(x => x.SeedData.Order)
            .ToArray();

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var authContext = services.GetRequiredService<AuthDbContext>();
        await authContext.Database.MigrateAsync();
        await ApplySeedingAsync(authContext, seedDataInfos);

        var dataContext = services.GetRequiredService<MaetsDbContext>();
        await dataContext.Database.MigrateAsync();
        await ApplySeedingAsync(dataContext, seedDataInfos);
    }

    private static async Task ApplySeedingAsync(DbContext context, SeedDataInfo[] seedDataInfos)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var seedDataInstances = seedDataInfos
            .Where(x => context.Model.FindEntityType(x.EntityType) is not null)
            .Select(x => x.SeedData);

        foreach (ISeedData seedData in seedDataInstances)
        {
            await seedData.ApplyAsync(context);
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
