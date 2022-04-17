using System.Text;

namespace Maets.Helpers;

public class CssClassBuilder
{
    private readonly StringBuilder _builder;

    public CssClassBuilder()
    {
        _builder = new StringBuilder();
    }

    public CssClassBuilder(string initialClass)
    {
        _builder = new StringBuilder(initialClass);
    }

    public CssClassBuilder Add(string cssClass)
    {
        _builder.Append(' ');
        _builder.Append(cssClass);

        return this;
    }

    public CssClassBuilder AddIf(bool condition, string cssClass)
    {
        return condition ? Add(cssClass) : this;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
