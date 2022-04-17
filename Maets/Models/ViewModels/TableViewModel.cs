using Maets.Domain.Entities;
using Maets.Extensions;
using Microsoft.AspNetCore.Html;

namespace Maets.Models.ViewModels;

public abstract class TableViewModel
{
    public bool CanCreate { get; set; } = true;
    public bool CanEdit { get; set; } = true;
    public bool CanViewDetails { get; set; } = false;
    public bool CanDelete { get; set; } = true;

    public abstract IEnumerable<object> GetItems();
    public abstract IEnumerable<Column> GetColumns();
    public abstract Guid GetItemId(object item);

    public bool HasActions => CanCreate || CanEdit;

    public abstract class Column
    {
        public string Header { get; init; } = string.Empty;
        public abstract IHtmlContent GetDisplay(object item);
    }
}

public class TableViewModel<TItem> : TableViewModel
{
    public Func<TItem, Guid>? ItemIdSelector { get; init; }

    public IEnumerable<TItem> Items { get; init; } = Array.Empty<TItem>();

    public IEnumerable<Column> Columns { get; init; } = Array.Empty<Column>();

    public new class Column : TableViewModel.Column
    {
        public Func<TItem, string?>? ValueSelector { get; init; }

        public HtmlRenderer<TItem>? ContentSelector { get; init; }

        public override IHtmlContent GetDisplay(object item)
        {
            return ContentSelector is not null
                ? ContentSelector((TItem)item)
                : new HtmlString(ValueSelector?.Invoke((TItem)item) ?? string.Empty);
        }
    }

    public override IEnumerable<object> GetItems()
    {
        return Items.Cast<object>();
    }

    public override IEnumerable<TableViewModel.Column> GetColumns()
    {
        return Columns;
    }
    public override Guid GetItemId(object item)
    {
        if (ItemIdSelector is null)
        {
            throw new ArgumentNullException();
        }
        return ItemIdSelector((TItem) item);
    }
}
