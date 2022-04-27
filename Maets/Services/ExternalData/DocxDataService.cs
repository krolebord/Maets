using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Maets.Attributes;
using Maets.Models.ExternalData;
using Newtonsoft.Json;

namespace Maets.Services.ExternalData;

[Dependency]
public class DocxDataService
{
    public void ExportDocumentToStream(Stream stream, CommonTable dataTable)
    {
        if (!dataTable.Validate())
        {
            throw new ArgumentException("Invalid table", nameof(dataTable));
        }
        
        var table = new Table();
        
        var props = new TableProperties(
        new TableBorders(
            new TopBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            },
            new BottomBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            },
            new LeftBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            },
            new RightBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            },
            new InsideHorizontalBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            },
            new InsideVerticalBorder
            {
                Val = new EnumValue<BorderValues>(BorderValues.Single),
                Size = 12
            }));

        table.AppendChild(props);

        var headerRow = new TableRow();

        foreach (var column in dataTable.Columns)
        {
            var cell = new TableCell();
            
            cell.Append(new Paragraph(new Run(new Text(column.Title))));

            headerRow.AppendChild(cell);
        }

        table.AppendChild(headerRow);

        foreach (var row in dataTable.Rows)
        {
            var tableRow = new TableRow();
            foreach (var item in row.Cells)
            {
                var cell = new TableCell();
                cell.Append(new Paragraph(new Run(new Text(item))));

                tableRow.AppendChild(cell);
            }
            table.AppendChild(tableRow);
        }

        using var wordprocessingDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);

        var mainDocumentPart = wordprocessingDocument.AddMainDocumentPart();

        mainDocumentPart.Document = new Document(new Body(table));
        
        wordprocessingDocument.Close();
    }

    public CommonTable ImportFromDocumentStream(Stream stream)
    {
        using var wordprocessingDocument = WordprocessingDocument.Open(stream, false);
        var table = wordprocessingDocument.MainDocumentPart!.Document.Body!
            .Elements<Table>()
            .First();
        
        var worksheetColumns = table.Elements<TableRow>()
            .First()
            .Descendants<TableCell>()
            .Select(x => x.Descendants<Text>().First().Text)
            .TakeWhile(x => !string.IsNullOrWhiteSpace(x));

        var commonTable = new CommonTable
        {
            Columns = worksheetColumns
                .Select(x => new CommonTable.Column(x))
                .ToList(),
            Rows = new List<CommonTable.Row>()
        };

        foreach (var row in table.Elements<TableRow>().Skip(1))
        {
            var rowData = row.Descendants<TableCell>()
                .Select(cell => cell.Descendants<Text>().First().Text)
                .ToList();
            
            commonTable.Rows.Add(new CommonTable.Row
            {
                Cells = rowData
            });
        }

        return commonTable;
    }
}
