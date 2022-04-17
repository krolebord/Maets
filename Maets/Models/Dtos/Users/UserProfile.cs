using AutoMapper;
using Maets.Domain.Entities;
using Maets.Extensions;

namespace Maets.Models.Dtos.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserReadDto>()
            .MapAvatarUrl(dto => dto.AvatarUrl, user => user.Avatar);
    }
}
