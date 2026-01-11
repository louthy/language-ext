using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using L = LanguageExt;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<A> Set<A>() =>
        L.Set.create<A>();

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<A> Set<A>(A head, params A[] tail) =>
        L.Set.createRange(head.Cons(tail));

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<A> toSet<A>(IEnumerable<A> items) =>
        items is Set<A> s
            ? s
            : L.Set.createRange(items);

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<A> toSet<A>(Iterator<A> items) =>
        new(items);
    
    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<T> toSet<T>(ReadOnlySpan<T> items) =>
        [..items];


    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> Set<OrdT, T>() where OrdT : Ord<T> =>
        L.Set.create<OrdT, T>();

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> Set<OrdT, T>(T head, params T[] tail) where OrdT : Ord<T> =>
        L.Set.createRange<OrdT, T>(head.Cons(tail));

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> toSet<OrdT, T>(IEnumerable<T> items) where OrdT : Ord<T> =>
        items is Set<OrdT, T> s
            ? s
            : L.Set.createRange<OrdT, T>(items);

    /// <summary>
    /// Create an immutable set
    /// </summary>
    [Pure]
    public static Set<OrdT, T> toSet<OrdT, T>(ReadOnlySpan<T> items) where OrdT : Ord<T> =>
        [..items];
}
