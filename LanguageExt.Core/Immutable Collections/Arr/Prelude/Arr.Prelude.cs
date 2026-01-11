using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LSeq = LanguageExt.Seq;
using L = LanguageExt;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<A> Arr<A>() =>
        L.Arr<A>.Empty;

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<A> Arr<A>(A x, params A[] xs) =>
        new (x.Cons(xs).ToArray());

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<A> toArr<A>(IEnumerable<A> items) =>
        items is Arr<A> arr
            ? arr
            : new Arr<A>(items);

    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<A> toArr<A>(Iterator<A> items)
    {
        var writer = ArrayWriter<A>.Init();
        foreach (var item in items)
        {
            writer.Add(item);
        }
        return writer.ToArr();
    }
    
    /// <summary>
    /// Create an immutable array
    /// </summary>
    [Pure]
    public static Arr<T> toArr<T>(ReadOnlySpan<T> items) =>
        new (items);
}
