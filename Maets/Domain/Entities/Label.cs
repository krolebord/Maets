namespace Maets.Domain.Entities;

public sealed class Label : Entity
{
    public string Name { get; set; } = null!;

    public ICollection<App> Apps { get; set; } = new HashSet<App>();
}
