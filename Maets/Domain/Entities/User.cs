namespace Maets.Domain.Entities;

public class User : Entity
{

    public string UserName { get; set; } = null!;

    public Guid? AvatarId { get; set; }

    public MediaFile? Avatar { get; set; }
}
