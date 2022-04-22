using Maets.Attributes;
using Maets.Data;
using Maets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maets.Services.Labels;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(ILabelsService))]
public class LabelsService : ILabelsService
{
    private readonly MaetsDbContext _context;

    public LabelsService(MaetsDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Label>> GetOrAddLabelsByNames(IEnumerable<string> labelNames)
    {
        labelNames = labelNames.ToArray();
        var existingLabels = await _context.Labels
            .Where(x => labelNames.Contains(x.Name))
            .ToListAsync();

        var newLabels = labelNames.Except(existingLabels.Select(x => x.Name))
            .Select(labelName => new Label
            {
                Id = Guid.NewGuid(),
                Name = labelName
            })
            .ToArray();

        _context.Labels.AddRange(newLabels);
        await _context.SaveChangesAsync();

        existingLabels.AddRange(newLabels);
        return existingLabels;
    }
}
