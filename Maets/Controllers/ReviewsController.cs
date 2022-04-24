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
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return RedirectToPage("/store/review", new { Id = id.Value });
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

            var review = await _context.Reviews
                .Include(x => x.App)
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id.Value);
            
            return View(_mapper.Map<ReviewWriteDto>(review));
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, ReviewWriteDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return View(reviewDto);
            }
            
            var review = await _context.Reviews
                .Include(x => x.App)
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (review is null)
            {
                throw new NotFoundException<Review>();
            }

            _mapper.Map(reviewDto, review);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is not null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
