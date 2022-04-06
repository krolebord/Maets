namespace Maets.Domain.Entities;

public sealed class AppScreenshot : Entity
{
    public Guid FileId { get; set; }
    public Guid AppId { get; set; }

    public App? App { get; set; }
    public MediaFile? File { get; set; }
}
