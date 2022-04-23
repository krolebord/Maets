using System.ComponentModel.DataAnnotations;

namespace Maets.Models.Dtos.Users;

public class UserWriteDto
{
    [Required]
    public string UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public bool IsManager { get; set; } = false;

    public UserWriteDto()
    {
        UserName = string.Empty;
    }

    public UserWriteDto(string userName)
    {
        UserName = userName;
    }
}
