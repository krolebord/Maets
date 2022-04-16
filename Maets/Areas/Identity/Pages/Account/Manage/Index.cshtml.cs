// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Maets.Extensions;
using Maets.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Maets.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUsersService usersService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usersService = usersService;
        }

        public string AvatarUrl { get; set; } = string.Empty;

        [TempData]
        public string? StatusMessage { get; set; }


        [BindProperty]
        public UserNameModel UserNameInput { get; set; } = null!;

        public class UserNameModel
        {
            [Required]
            [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
            [Display(Name = "User name")]
            public string UserName { get; set; } = null!;
        }

        [BindProperty]
        public AvatarModel AvatarInput { get; set; } = null!;

        public class AvatarModel
        {
            public IFormFile? Avatar { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            UserNameInput = new UserNameModel
            {
                UserName = userName
            };

            AvatarUrl = await _usersService.GetAvatarUrl(Guid.Parse(user.Id));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostUserNameChangeAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(UserNameInput.UserName) || UserNameInput.UserName == user.UserName)
            {
                StatusMessage = "Warning Your email is unchanged.";
                return Page();
            }

            if (await _userManager.FindByNameAsync(UserNameInput.UserName) is not null)
            {
                StatusMessage = "Error This user name is already taken";
                return Page();
            }

            await _usersService.UpdateUserName(Guid.Parse(user.Id), UserNameInput.UserName);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAvatarChangeAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (AvatarInput.Avatar is null)
            {
                StatusMessage = "Error Avatar file is not specified";
                return Page();
            }

            await _usersService.UpdateUserAvatar(Guid.Parse(user.Id), AvatarInput.Avatar);
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToPage();
        }
    }
}
