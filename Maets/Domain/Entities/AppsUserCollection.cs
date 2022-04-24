namespace Maets.Domain.Entities;

public class AppsUserCollection : Entity
{
    public Guid AppId { get; set; }
    public Guid UserId { get; set; }

    public App? App { get; set; }
    public User? User { get; set; }
}
