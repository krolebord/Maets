using Maets.Models.Dtos.Shared;

namespace Maets.Models.Dtos.Users;

public class UserShortDto : EntityDto
{
    public string UserName { get; set; } = string.Empty;
}
