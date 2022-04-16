// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Maets.Domain.Entities.Identity;
using Maets.Services;
using Maets.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Maets.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ConfirmEmailChangeModel(SignInManager<ApplicationUser> signInManager, IUsersService usersService, UserManager<ApplicationUser> userManager)
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
