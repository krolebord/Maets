using ClosedXML.Excel;
using Maets.Attributes;
using Maets.Models.ExternalData;

namespace Maets.Services.ExternalData;

[Dependency]
public class ExcelDataService
{
    public void ExportSpreadsheetToStream(Stream stream, CommonTable table)
    {
        if (!table.Validate())
        {
            throw new ArgumentException("Invalid table", nameof(table));
        }
        
        var workbook = new XLWorkbook(XLEventTracking.Disabled);
        var worksheet = workbook.AddWorksheet(table.Title);

        for (var i = 0; i < table.Columns.Count; ++i)
        {
            var column = table.Columns[i];
            worksheet.Cell(1, i + 1).SetValue(column.Title);
        }
        
        for (var rowIndex = 0; rowIndex < table.Rows.Count; ++rowIndex)
        {
            var row = table.Rows[rowIndex];
            for (int cellIndex = 0; cellIndex < row.Cells.Count; ++cellIndex)
            {
                var value = row.Cells[cellIndex] ?? string.Empty;
                worksheet.Cell(rowIndex + 2, cellIndex + 1).SetValue(value);
            }
        }
        
        workbook.SaveAs(stream);
    }

    public CommonTable ImportFromSpreadsheetStream(Stream stream)
    {
        using var workbook = new XLWorkbook(stream, XLEventTracking.Disabled);
        var worksheet = workbook.Worksheets.First();
        var worksheetColumns = worksheet
            .Row(1)
            .Cells()
            .Select(x => x.GetString())
            .TakeWhile(x => !string.IsNullOrWhiteSpace(x));

        var table = new CommonTable
        {
            Columns = worksheetColumns
                .Select(x => new CommonTable.Column(x))
                .ToList(),
            Rows = new List<CommonTable.Row>()
        };

        foreach (var row in worksheet.Rows().Skip(1))
        {
            var rowData = new List<object?>();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                rowData.Add(row.Cell(columnIndex + 1).Value);
            }
            table.Rows.Add(new CommonTable.Row
            {
                Cells = rowData
            });
        }
        
        return table;
    }
}
