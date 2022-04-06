namespace Maets.Domain.Entities;

public sealed class AppsLabel : Entity
{
    public Guid AppId { get; set; }
    public Guid LabelId { get; set; }

    public App? App { get; set; }
    public Label? Label { get; set; }
}
