namespace Maets.Models.Exceptions;

public class NotFoundException : ServiceException
{
    public NotFoundException(string entity)
        : base($"{entity} was not found")
    {}

    public NotFoundException(Type entityType)
        : this(entityType.Name)
    {}
}

public class NotFoundException<TEntity> : NotFoundException
{
    public NotFoundException() : base(typeof(TEntity))
    {}
}
