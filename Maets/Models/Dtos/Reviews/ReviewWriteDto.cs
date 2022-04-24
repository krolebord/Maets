namespace Maets.Models.Dtos.Reviews;

public class ReviewWriteDto
{
    public int Score { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid AppId { get; set; }
}
