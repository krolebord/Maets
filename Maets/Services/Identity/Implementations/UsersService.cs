using System.Text;
using System.Text.Encodings.Web;
using AutoMapper;
using Maets.Attributes;
using Maets.Data;
using Maets.Domain.Constants;
using Maets.Domain.Entities;
using Maets.Domain.Entities.Identity;
using Maets.Extensions;
using Maets.Models.Dtos.Users;
using Maets.Models.Exceptions;
using Maets.Services.Files;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Maets.Services.Identity.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IUsersService))]
internal class UsersService : IUsersService
{
    private readonly AuthDbContext _authDbContext;
    private readonly MaetsDbContext _maetsDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailSender _emailSender;
    private readonly IFileReadService _fileReadService;
    private readonly IFileWriteService _fileWriteService;
    private readonly IMapper _mapper;

    public UsersService(
        MaetsDbContext maetsDbContext,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender,
        IFileReadService fileReadService,
        IFileWriteService fileWriteService,
        AuthDbContext authDbContext,
        IMapper mapper)
    {
        _maetsDbContext = maetsDbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _emailSender = emailSender;
        _fileReadService = fileReadService;
        _fileWriteService = fileWriteService;
        _authDbContext = authDbContext;
        _mapper = mapper;
        _urlHelper = new UrlHelper(new ActionContext(_httpContextAccessor.HttpContext!, new RouteData(), new ActionDescriptor()));
    }

    public async Task<IEnumerable<UserForAdminDto>> GetForAdmin()
    {
        var users = await _maetsDbContext.Users
            .Include(x => x.Avatar)
            .ToListAsync();

        var identityUsers = await _authDbContext.Users
            .Include(x => x.Roles)
            .ToListAsync();

        return users.Join(
            identityUsers,
            user => user.Id.ToString(),
            identityUser => identityUser.Id,
            (user, applicationUser) => new UserForAdminDto(
                user.Id,
                user.UserName,
                applicationUser.Email,
                _fileReadService.AvatarUrlOrDefault(user.Avatar),
                applicationUser.Roles.Select(x => x.Name).ToArray()
            )
        );
    }

    public async Task<UserReadDto?> Find(Guid userId)
    {
        var user = await _maetsDbContext.Users
            .Include(x => x.Avatar)
            .FirstOrDefaultAsync(x => x.Id == userId);

        var identityUser = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null || identityUser is null)
        {
            return null;
        }

        return _mapper.Map<UserReadDto>(user);
    }

    public async Task<IdentityResult> CreateUser(UserWriteDto userWriteDto, string password)
    {
        var userId = Guid.NewGuid();

        var identityUser = new ApplicationUser()
        {
            Id = userId.ToString(),
            UserName = userWriteDto.UserName,
            Email = userWriteDto.Email
        };
        var result = await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            return result;
        }

        if (userWriteDto.IsManager)
        {
            await _userManager.AddToRoleAsync(identityUser, RoleNames.Moderator);
        }

        var user = new User
        {
            Id = userId,
            UserName = userWriteDto.UserName
        };

        _maetsDbContext.Users.Add(user);
        await _maetsDbContext.SaveChangesAsync();

        return result;
    }

    public async Task UpdateUserName(Guid userId, string newUserName)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());

        if (identityUser is null)
        {
            throw new NotFoundException<User>();
        }

        await _userManager.SetUserNameAsync(identityUser, newUserName);

        var user = await _maetsDbContext.Users.FirstAsync(x => x.Id == userId);
        user.UserName = newUserName;
        await _maetsDbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAvatar(Guid userId, IFormFile avatarFile)
    {
        var user = await _maetsDbContext.Users
            .Include(x => x.Avatar)
            .FirstOrNotFound(x => x.Id == userId);

        if (user.Avatar is not null)
        {
            await _fileWriteService.DeleteFileAsync(user.Avatar);
        }

        var avatarKey = $"avatars/{user.UserName}-{Guid.NewGuid()}.png";
        user.Avatar = await _fileWriteService.UploadFileAsync(avatarKey, avatarFile.OpenReadStream());

        await _maetsDbContext.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser is not null)
        {
            await _userManager.DeleteAsync(identityUser);
        }

        _maetsDbContext.Users.Remove(new User { Id = userId });
        await _maetsDbContext.SaveChangesAsync();
    }

    public async Task<string> GetAvatarUrl(Guid userId)
    {
        var avatarKey = await _maetsDbContext.Users
            .Include(x => x.Avatar)
            .Where(x => x.Id == userId && x.Avatar != null)
            .Select(x => x.Avatar!.Key)
            .FirstOrDefaultAsync();

        return _fileReadService.AvatarUrlOrDefault(avatarKey);
    }

    public async Task SendEmailConfirmation(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new NotFoundException<User>();
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _urlHelper.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new
            {
                area = "Identity",
                userId = user.Id,
                code = code,
                returnUrl = _urlHelper.Content("~/")
            },
            protocol: _httpContextAccessor.HttpContext?.Request.Scheme ?? "http"
        ) ?? string.Empty;

        await _emailSender.SendEmailAsync(
            user.Email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
        );
    }

    public async Task<IdentityResult> ConfirmEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        return await _userManager.ConfirmEmailAsync(user, code);
    }

    public async Task SendEmailChangeConfirmation(Guid userId, string newEmail)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new NotFoundException<User>();
        }

        var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = _urlHelper.Page(
            "/Account/ConfirmEmailChange",
            pageHandler: null,
            values: new
            {
                area = "Identity",
                userId = userId,
                email = newEmail,
                code = code
            },
            protocol: _httpContextAccessor.HttpContext?.Request.Scheme ?? "https"
        );

        await _emailSender.SendEmailAsync(
            newEmail,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>."
        );
    }

    public async Task<IdentityResult> ConfirmEmailChange(string userId, string email, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        return await _userManager.ChangeEmailAsync(user, email, code);
    }

    public async Task EnsureManager(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (!await _userManager.IsInRoleAsync(user, RoleNames.Moderator))
        {
            await _userManager.AddToRoleAsync(user, RoleNames.Moderator);
        }
    }

    public async Task EnsureNotManager(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (await _userManager.IsInRoleAsync(user, RoleNames.Moderator))
        {
            await _userManager.RemoveFromRoleAsync(user, RoleNames.Moderator);
        }
    }
}
