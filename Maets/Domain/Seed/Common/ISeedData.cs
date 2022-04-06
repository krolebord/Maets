using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed.Common;

public interface ISeedData
{
    public int Order { get; }

    public Task ApplyAsync(DbContext context);
}

public interface ISeedData<TEntity> : ISeedData where TEntity : class
{}
