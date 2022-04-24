using AutoMapper;
using Maets.Data;
using Maets.Domain.Entities;
using Maets.Models.Dtos.Reviews;
using Maets.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Maets.Pages;

public class ReviewPage : PageModel
{
    private readonly MaetsDbContext _context;
    private readonly IMapper _mapper;

    public ReviewReadDto ReviewDto { get; set; } = null!;
    
    public ReviewPage(MaetsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task OnGetAsync(Guid id)
    {
        var review = await _context.Reviews
            .Include(x => x.App)
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (review is null)
        {
            throw new NotFoundException<Review>();
        }

        ReviewDto = _mapper.Map<ReviewReadDto>(review);
    }
}
