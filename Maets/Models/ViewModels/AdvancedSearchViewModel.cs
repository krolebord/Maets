using Maets.Domain.Entities;

namespace Maets.Models.ViewModels;

public record SearchResultDto(string Name, string Url);

public record AdvancedSearchViewModel(string Sql, IEnumerable<SearchResultDto> Items);
