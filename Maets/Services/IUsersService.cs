using Maets.Models.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace Maets.Services;

public interface IUsersService
{
    Task<UserReadDto?> Find(Guid userId);
    Task<IdentityResult> CreateUser(UserWriteDto userWriteDto, string password);
    Task DeleteUser(Guid userId);

    Task SendEmailConfirmation(Guid userId);
    Task<IdentityResult> ConfirmEmail(string userId, string code);

    Task SendEmailChangeConfirmation(Guid userId, string newEmail);
    Task<IdentityResult> ConfirmEmailChange(string userId, string email, string code);
}
