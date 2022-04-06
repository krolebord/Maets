namespace Maets.Domain.Entities;

public sealed class Review : Entity
{

    public int Score { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public Guid AppId { get; set; }


    public App? App { get; set; }

    public User? Author { get; set; }
}
