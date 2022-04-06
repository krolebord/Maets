namespace Maets.Domain.Entities;

public sealed class AppsDeveloper : Entity
{
    public Guid AppId { get; set; }
    public Guid CompanyId { get; set; }

    public App? App { get; set; }
    public Company? Company { get; set; }
}
