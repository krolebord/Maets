using Maets.Attributes;
using Maets.Models.ExternalData;

namespace Maets.Services.ExternalData;

[Dependency]
public class DataTransformationService
{
    public CommonTable ToTableData<TItem>(IEnumerable<TItem> items)
    {
        var properties = typeof(TItem).GetProperties();
        
        var table = new CommonTable
        {
            Title = typeof(TItem).Name,
            Columns = properties
                .Select(x => new CommonTable.Column(x.Name))
                .ToList(),
            Rows = new List<CommonTable.Row>()
        };

        foreach (var item in items)
        {
            var row = new CommonTable.Row
            {
                Cells = properties
                    .Select(x => x.GetValue(item))
                    .ToList()
            };

            table.Rows.Add(row);
        }

        return table;
    }
    
    public IEnumerable<TItem> FromTableData<TItem>(CommonTable table)
    {
        var properties = typeof(TItem).GetProperties();
        
        if (!table.Validate() || properties.Length != table.Columns.Count)
        {
            throw new ArgumentException("Invalid table", nameof(table));
        }
        
        var items = table.Rows
            .Select(_ => Activator.CreateInstance<TItem>())
            .ToList();

        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
        {
            var column = table.Columns[columnIndex];
            
            var property = properties.FirstOrDefault(x => x.Name == column.Title);
            if (property is null)
            {
                throw new ArgumentException($"Invalid column: {column.Title}", nameof(table));
            }

            for (int rowIndex = 0; rowIndex < items.Count; rowIndex++)
            {
                property.SetValue(items[rowIndex], table.Rows[rowIndex].Cells[columnIndex]);
            }
        }

        return items;
    }
}
