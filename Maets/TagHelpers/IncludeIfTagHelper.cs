using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Maets.TagHelpers;

[HtmlTargetElement("*", Attributes = "include-if")]
public class IncludeIfTagHelper : TagHelper
{
    [HtmlAttributeName("include-if")]
    public bool Include { get; set; } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!Include)
        {
            output.TagName = null;
        }
    }
}
