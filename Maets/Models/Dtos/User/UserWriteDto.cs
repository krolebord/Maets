using System.ComponentModel.DataAnnotations;

namespace Maets.Models.Dtos.User;

public class UserWriteDto
{
    [Required]
    public string UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public Guid? AvatarId { get; set; }

    public UserWriteDto()
    {
        UserName = string.Empty;
    }

    public UserWriteDto(string userName)
    {
        UserName = userName;
    }
}
