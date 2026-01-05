namespace SchemaSharp;

internal static class EnumerableExtensions
{
    public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> source, CancellationToken? cancellationToken = null)
        where T : IEquatable<T>
    {
        cancellationToken?.ThrowIfCancellationRequested();

        if (source is T[] array)
        {
            return new(array);
        }

        return new EquatableArray<T>([.. source]);
    }
}