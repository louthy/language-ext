using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using L = LanguageExt;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> Lst<T>() =>
        L.Lst<T>.Empty;

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> Lst<T>(T x, params T[] xs)
    {
        return new Lst<T>(Yield());

        IEnumerable<T> Yield()
        {
            yield return x;
            foreach(var item in xs)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toLst<T>(Arr<T> items) =>
        new (items.AsSpan());

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toLst<T>(IEnumerable<T> items) =>
        items is Lst<T> lst
            ? lst
            : new Lst<T>(items);

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toLst<T>(Iterator<T> items) =>
        new (items);

    /// <summary>
    /// Create an immutable list
    /// </summary>
    [Pure]
    public static Lst<T> toLst<T>(ReadOnlySpan<T> items) =>
        new (items);
}
