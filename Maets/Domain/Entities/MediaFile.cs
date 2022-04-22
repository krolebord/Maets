namespace Maets.Domain.Entities;

public sealed class MediaFile : Entity
{
    public string Key { get; set; } = null!;

    private ICollection<App> Apps { get; set; } = new HashSet<App>();
}
