namespace Maets.Domain.Entities;

public sealed class Company : Entity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid? PhotoId { get; set; }

    public MediaFile? Photo { get; set; }

    public ICollection<App> PublishedApps { get; set; } = new HashSet<App>();

    public ICollection<App> DevelopedApps { get; set; } = new HashSet<App>();

    public ICollection<CompanyEmployee> Employees { get; set; } = new HashSet<CompanyEmployee>();
}
