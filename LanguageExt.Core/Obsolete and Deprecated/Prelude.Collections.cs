using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using L = LanguageExt;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class Prelude
{
    [Obsolete("Use Lst instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Lst<T> List<T>() =>
        L.Lst<T>.Empty;

    [Obsolete("Use Lst instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Lst<T> toList<T>(Arr<T> items) =>
        new (items.AsSpan());

    [Obsolete("Use toLst instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Lst<T> toList<T>(IEnumerable<T> items) =>
        items is Lst<T> lst
            ? lst
            : new Lst<T>(items);

    [Obsolete("Use toLst instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Lst<T> toList<T>(ReadOnlySpan<T> items) =>
        new (items);


    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Stck<T> Stack<T>() =>
        L.Stck<T>.Empty;

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Stck<T> Stck<T>(params T[] items) =>
        L.Stck.createRange(items);

    [Obsolete("Use toStck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Stck<T> toStack<T>(IEnumerable<T> items) =>
        L.Stck.createRange(items);
}
