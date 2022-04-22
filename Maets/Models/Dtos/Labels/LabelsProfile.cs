using AutoMapper;
using Maets.Domain.Entities;

namespace Maets.Models.Dtos.Labels;

public class LabelsProfile : Profile
{
    public LabelsProfile()
    {
        CreateMap<Label, string>()
            .ConstructUsing(x => x.Name);
    }
}
