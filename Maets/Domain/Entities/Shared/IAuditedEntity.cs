namespace Maets.Domain.Entities;

public interface IAuditedEntity
{
    DateTimeOffset CreationDate { get; set; }
}
