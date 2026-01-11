using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using LSeq = LanguageExt.Seq;
using L = LanguageExt;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

public static partial class Prelude
{
    [Obsolete("Use `Arr` instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Arr<A> Array<A>() =>
        L.Arr<A>.Empty;

    [Obsolete("Use `Arr` instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Arr<A> Array<A>(A x, params A[] xs) =>
        new (x.Cons(xs).ToArray());

    [Obsolete("Use `toArr` instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Arr<A> toArray<A>(IEnumerable<A> items) =>
        items is Arr<A> arr
            ? arr
            : new Arr<A>(items);

    [Obsolete("Use `toArr` instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Arr<A> toArray<A>(Iterator<A> items)
    {
        var writer = ArrayWriter<A>.Init();
        foreach (var item in items)
        {
            writer.Add(item);
        }
        return writer.ToArr();
    }
    
    [Obsolete("Use `toArr` instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Arr<T> toArray<T>(ReadOnlySpan<T> items) =>
        new (items);
}
