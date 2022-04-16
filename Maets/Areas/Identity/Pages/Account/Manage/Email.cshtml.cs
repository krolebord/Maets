// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Maets.Domain.Entities.Identity;
using Maets.Services;
using Maets.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Maets.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _usersService;

        public EmailModel(
            UserManager<ApplicationUser> userManager,
            IUsersService usersService)
        {
            _userManager = userManager;
            _usersService = usersService;
        }


        public string? Email { get; set; }


        public bool IsEmailConfirmed { get; set; }


        [TempData]
        public string? StatusMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; } = null!;


        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string? NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel();

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
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

        public async Task<IActionResult> OnPostChangeEmailAsync()
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

            var email = await _userManager.GetEmailAsync(user);

            if (string.IsNullOrWhiteSpace(Input.NewEmail) || Input.NewEmail == email)
            {
                StatusMessage = "Warning Your email is unchanged.";
                return RedirectToPage();
            }

            var userId = Guid.Parse(await _userManager.GetUserIdAsync(user));

            await _usersService.SendEmailChangeConfirmation(userId, Input.NewEmail);

            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            await _usersService.SendEmailConfirmation(Guid.Parse(user.Id));

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
