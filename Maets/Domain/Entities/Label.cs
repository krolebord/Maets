namespace Maets.Domain.Entities;

public sealed class Label : Entity
{
    public string Name { get; set; } = null!;

    public ICollection<AppsLabel> AppsLabels { get; set; } = new HashSet<AppsLabel>();

    public ICollection<App> Apps { get; set; } = new HashSet<App>();
}
