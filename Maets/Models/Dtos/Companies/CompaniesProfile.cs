using AutoMapper;
using Maets.Domain.Entities;
using Maets.Extensions;

namespace Maets.Models.Dtos.Companies;

public class CompaniesProfile : Profile
{
    public CompaniesProfile()
    {
        CreateMap<Company, CompanyReadDto>()
            .MapImageUrl(dto => dto.PhotoUrl, company => company.Photo);

        CreateMap<Company, CompanyShortDto>();
    }
}
