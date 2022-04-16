using System.Linq.Expressions;
using Maets.Domain.Entities;
using Maets.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Maets.Extensions;

public static class DbContextExtensions
{
    public static void RemoveEntity<TEntity>(this DbSet<TEntity> set, Guid id)
        where TEntity : Entity, new()
    {
        set.Remove(new TEntity { Id = id });
    }

    public static async Task<TEntity> FirstOrNotFound<TEntity>(this IQueryable<TEntity> set)
        where TEntity : class
    {
        return await set.FirstOrDefaultAsync() ?? throw new NotFoundException<TEntity>();
    }

    public static async Task<TEntity> FirstOrNotFound<TEntity>(this IQueryable<TEntity> set, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return await set.FirstOrDefaultAsync(predicate) ?? throw new NotFoundException<TEntity>();
    }
}
