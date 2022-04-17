using Microsoft.AspNetCore.Html;

namespace Maets.Extensions;

public delegate IHtmlContent HtmlRenderer<in TItem>(TItem item);
