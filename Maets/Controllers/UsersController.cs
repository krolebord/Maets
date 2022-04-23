using Maets.Data;
using Maets.Domain.Constants;
using Maets.Extensions;
using Maets.Models.Dtos.Users;
using Maets.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Maets.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class Users : MaetsController
    {
        private readonly IUsersService _usersService;
        private readonly MaetsDbContext _context;

        public Users(MaetsDbContext context, IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _usersService.GetForAdmin();

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["AvatarId"] = new SelectList(_context.MediaFiles, "Id", "Id");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserWriteDto user)
        {
            if (!ModelState.IsValid)
                return View(user);

            await _usersService.CreateUser(user, user.UserName);

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserWriteDto(user.UserName));
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UserWriteDto user)
        {
            if (!ModelState.IsValid)
                return View(user);

            await _usersService.UpdateUserName(id, user.UserName);

            if (user.IsManager)
            {
                await _usersService.EnsureManager(id);
            }
            else
            {
                await _usersService.EnsureNotManager(id);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _usersService.Find(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (User.GetId() == id)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FindAsync(id);

            if (user is null)
                return RedirectToAction(nameof(Index));

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
