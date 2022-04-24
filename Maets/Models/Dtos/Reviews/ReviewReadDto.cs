using Maets.Models.Dtos.Apps;
using Maets.Models.Dtos.Shared;
using Maets.Models.Dtos.Users;

namespace Maets.Models.Dtos.Reviews;

public class ReviewReadDto : EntityDto
{
    public int Score { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTimeOffset CreationDate { get; set; }

    public UserShortDto Author { get; set; } = null!;

    public AppShortDto App { get; set; } = null!;
}
