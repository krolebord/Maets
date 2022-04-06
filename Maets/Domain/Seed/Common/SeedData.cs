using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed.Common;

public abstract class SeedData<TEntity> : ISeedData<TEntity> where TEntity : class
{
    public virtual int Order => 0;

    protected abstract Task<bool> CheckIfInDatabaseAsync(DbSet<TEntity> set, TEntity entity);

    protected abstract Task<IEnumerable<TEntity>> GetEntities(DbContext context);

    public async Task ApplyAsync(DbContext context)
    {
        var seedEntities = await GetEntities(context);
        var set = context.Set<TEntity>();

        foreach (var entity in seedEntities)
        {
            if (await CheckIfInDatabaseAsync(set, entity))
            {
                continue;
            }

            set.Add(entity);
        }

        await context.SaveChangesAsync();
    }
}
