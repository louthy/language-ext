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
    [Obsolete("Use Lst instead")]
    public static Lst<T> List<T>() =>
        L.Lst<T>.Empty;

    [Obsolete("Use Lst instead")]
    public static Lst<T> List<T>(T x, params T[] xs)
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

    [Obsolete("Use toLst instead")]
    public static Lst<T> toList<T>(Arr<T> items) =>
        new (items.AsSpan());

    [Obsolete("Use toLst instead")]
    public static Lst<T> toList<T>(IEnumerable<T> items) =>
        items is Lst<T> lst
            ? lst
            : new Lst<T>(items);

    [Obsolete("Use toLst instead")]
    public static Lst<T> toList<T>(ReadOnlySpan<T> items) =>
        new (items);
}
