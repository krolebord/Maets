using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Extensions;
using Maets.Models.Dtos.Reviews;
using Maets.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Maets.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MaetsDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsController(MaetsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.App)
                .Include(r => r.Author)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();
            var dtos = _mapper.Map<List<ReviewReadDto>>(reviews);
            return View(dtos);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.App)
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ReviewReadDto>(review));
        }

        // GET: Reviews/Create
        [Authorize]
        public async Task<IActionResult> Create(Guid id)
        {
            if (!await _context.Apps.AnyAsync(x => x.Id == id))
            {
                throw new NotFoundException<App>();
            }
            
            if (await _context.Reviews.AnyAsync(x => x.AppId == id && x.AuthorId == User.GetId()))
            {
                return View("AlreadyReviewed");
            }
            
            return View(new ReviewWriteDto
            {
                AppId = id
            });
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Guid id, ReviewWriteDto reviewDto)
        {
            if (!ModelState.IsValid || id != reviewDto.AppId)
            {
                return View(reviewDto);
            }
            
            if (!await _context.Apps.AnyAsync(x => x.Id == id))
            {
                throw new NotFoundException<App>();
            }

            if (await _context.Reviews.AnyAsync(x => x.AppId == id && x.AuthorId == User.GetId()))
            {
                return View("AlreadyReviewed");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Title = reviewDto.Title,
                Description = reviewDto.Description,
                Score = reviewDto.Score,
                AppId = reviewDto.AppId,
                AuthorId = User.GetId()
            };
            
            _context.Add(review);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/store/app", new { Id = id });
        }

        // GET: Reviews/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["AppId"] = new SelectList(_context.Apps, "Id", "Id", review.AppId);
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, [Bind("Score,Title,Description,AuthorId,AppId,Id")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["AppId"] = new SelectList(_context.Apps, "Id", "Id", review.AppId);
                ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
                return View(review);
            }
            
            _context.Update(review);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.App)
                .Include(r => r.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
