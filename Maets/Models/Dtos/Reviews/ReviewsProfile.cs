using AutoMapper;
using Maets.Domain.Entities;

namespace Maets.Models.Dtos.Reviews;

public class ReviewsProfile : Profile
{
    public ReviewsProfile()
    {
        CreateMap<Review, ReviewReadDto>();
        
        CreateMap<Review, ReviewWriteDto>()
            .ReverseMap();
    }
}
