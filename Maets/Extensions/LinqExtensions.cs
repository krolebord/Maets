namespace Maets.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<(T, int)> Indexed<T>(this IEnumerable<T> source)
    {
        return source.Select((x, i) => (x, i));
    }
}
