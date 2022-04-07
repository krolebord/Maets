using System.Reflection;
using Maets.Data;
using Maets.Domain.Seed.Common;
using Microsoft.EntityFrameworkCore;

namespace Maets.Extensions;

public static class SeedingExtensions
{
    private record struct SeedDataInfo(Type EntityType, Type SeedDataType);

    public static IServiceCollection AddSeedData(this IServiceCollection services, Assembly assembly)
    {
        var seedDataInfos = GetSeedDataInfos(assembly);

        services.AddSingleton<SeedDataInfo[]>(seedDataInfos);

        foreach (var seedDataInfo in seedDataInfos)
        {
            services.AddTransient(seedDataInfo.SeedDataType);
        }

        return services;
    }

    public static async Task SeedAndMigrateAsync(this WebApplication app)
    {
        var seedDataInfos = app.Services.GetRequiredService<SeedDataInfo[]>();

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var authContext = services.GetRequiredService<AuthDbContext>();
        await authContext.Database.MigrateAsync();
        await ApplySeedingAsync(services, authContext, seedDataInfos);

        var dataContext = services.GetRequiredService<MaetsDbContext>();
        await dataContext.Database.MigrateAsync();
        await ApplySeedingAsync(services, dataContext, seedDataInfos);
    }

    private static SeedDataInfo[] GetSeedDataInfos(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .Select(t => new
            {
                EntityType = t.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeedData<>))
                    ?.GetGenericArguments().FirstOrDefault(),
                SeedDataType = t
            })
            .Where(x => x.EntityType is not null)
            .Select(x => new SeedDataInfo(x.EntityType!, x.SeedDataType))
            .ToArray();
    }

    private static async Task ApplySeedingAsync(IServiceProvider services, DbContext context, SeedDataInfo[] seedDataInfos)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var seedDataInstances = seedDataInfos
            .Where(x => context.Model.FindEntityType(x.EntityType) is not null)
            .Select(x => (ISeedData)services.GetRequiredService(x.SeedDataType));

        foreach (ISeedData seedData in seedDataInstances)
        {
            await seedData.ApplyAsync(context);
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
