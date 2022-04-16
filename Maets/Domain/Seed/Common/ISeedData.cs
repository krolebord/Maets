using Microsoft.EntityFrameworkCore;

namespace Maets.Domain.Seed.Common;

public interface ISeedData
{
    public int Order { get; }

    public Task ApplyAsync(DbContext context);
}

// ReSharper disable once UnusedTypeParameter
public interface ISeedData<TEntity> : ISeedData where TEntity : class
{}
