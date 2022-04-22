using Maets.Domain.Entities;

namespace Maets.Services.Labels;

public interface ILabelsService
{
    Task<IEnumerable<Label>> GetOrAddLabelsByNames(IEnumerable<string> labelNames);
}
