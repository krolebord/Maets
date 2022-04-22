using System.Reflection;

namespace Maets.Helpers;

public static class EmbeddedResourceHelper
{
    public static string ReadEmbeddedResourceAsString(string fileName, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        var resourcePath = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(fileName));

        using Stream? stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
        return reader.ReadToEnd();
    }
}
