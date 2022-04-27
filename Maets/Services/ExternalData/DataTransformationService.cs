using System.Reflection;
using Maets.Attributes;
using Maets.Models.ExternalData;
using Newtonsoft.Json;

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
                    .Select(x => {
                        var value = x.GetValue(item);

                        if (value is string stringValue)
                        {
                            return stringValue;
                        }
                        
                        var serializedValue = JsonConvert.SerializeObject(value);
                        return serializedValue;
                    })
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
                var value = table.Rows[rowIndex].Cells[columnIndex];

                var targetType = property.PropertyType;
                
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(items[rowIndex], value);
                }
                else
                {
                    var convertedValue = JsonConvert.DeserializeObject(value, targetType);
                    
                    if (convertedValue is null)
                    {
                        throw new NotSupportedException($"Property type: {property.PropertyType.Name} is not supported");
                    }

                    property.SetValue(items[rowIndex], convertedValue);
                }
            }
        }

        return items;
    }
}
