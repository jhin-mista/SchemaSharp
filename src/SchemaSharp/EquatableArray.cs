// Adapted from Andrew Lock's EquatableArray implementation
// https://github.com/andrewlock/StronglyTypedId/blob/6bd17db4a4b700eaad9e209baf41478cc3f0bbe9/src/StronglyTypedIds/EquatableArray.cs
using System.Collections;

namespace SchemaSharp;

/// <summary>
/// An immutable, equatable array. This is equivalent to <see cref="IEnumerable{T}"/> but with value equality support.
/// </summary>
/// <typeparam name="T">The type of values in the array.</typeparam>
/// <remarks>
/// Creates a new <see cref="EquatableArray{T}"/> instance.
/// </remarks>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    internal readonly T[] Array => field ?? [];

    /// <summary>
    /// Contains a reference to an empty <see cref="EquatableArray{T}"/>.
    /// </summary>
    public static readonly EquatableArray<T> Empty = new([]);

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableArray{T}"/> struct.
    /// </summary>
    /// <param name="array">The input <typeparamref name="T"/>[] to wrap.</param>
    public EquatableArray(T[] array)
    {
        Array = array;
    }

    /// <inheritdoc/>
    public bool Equals(EquatableArray<T> array)
    {
        return AsSpan().SequenceEqual(array.AsSpan());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> array && Equals(array);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (Array is not T[] array)
        {
            return 0;
        }

        HashCode hashCode = default;

        foreach (T item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Returns a new <see cref="EquatableArray{T}"/> that contains all elements of the current array plus the specified item appended
    /// to the end.
    /// </summary>
    /// <remarks>This method does not modify the current array; instead, it returns a new instance with the
    /// added item. The equality semantics of <see cref="EquatableArray{T}"/> are preserved in the returned array.</remarks>
    /// <param name="item">The item to add to the end of the array.</param>
    /// <returns>A new <see cref="EquatableArray{T}"/> instance containing the original elements and the specified item as the last element.</returns>
    public EquatableArray<T> Add(T item)
    {
        if (Array.Length == 0)
        {
            return new([item]);
        }

        var array = new T[Array.Length + 1];
        System.Array.Copy(Array, array, Array.Length);

        var endIndex = array.Length - 1;
        array[endIndex] = item;

        return new(array);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
    public ReadOnlySpan<T> AsSpan()
    {
        return Array.AsSpan();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)(Array)).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Array.GetEnumerator();
    }

    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public int Count => Array?.Length ?? 0;

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
    /// </summary>
    /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
    /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
    /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
    {
        return !left.Equals(right);
    }
}