using System.Linq.Expressions;
using AutoMapper;
using Maets.Domain.Entities;
using Maets.Models.Dtos.MediaFiles;

namespace Maets.Extensions;

public static class AutomapperExtensions
{
    public static IMappingExpression<TSource, TDestination> MapAvatarUrl<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> map,
        Expression<Func<TDestination, string?>> urlSelector,
        Func<TSource, MediaFile?> fileSelector
    )
    {
        return map.ForMember(urlSelector, opt => opt.MapFrom(source => AvatarFileDto.From(fileSelector(source))));
    }

    public static IMappingExpression<TSource, TDestination> MapImageUrl<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> map,
        Expression<Func<TDestination, string>> urlSelector,
        Func<TSource, MediaFile?> fileSelector
    )
    {
        return map.ForMember(urlSelector, opt => opt.MapFrom(source => ImageFileDto.From(fileSelector(source))));
    }
}
