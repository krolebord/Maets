// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Maets.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Maets.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ConfirmEmailChangeModel(SignInManager<IdentityUser> signInManager, IUsersService usersService, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _usersService = usersService;
            _userManager = userManager;
        }


        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string? userId, string? email, string? code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _usersService.ConfirmEmailChange(userId, email, code);

            if (!result.Succeeded)
            {
                StatusMessage = "Error changing email.";
                return Page();
            }

            var user = await _userManager.FindByIdAsync(userId);
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Thank you for confirming your email change.";
            return Page();
        }
    }
}
