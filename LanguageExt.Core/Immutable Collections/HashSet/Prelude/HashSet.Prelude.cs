using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<A> HashSet<A>() =>
        L.HashSet.create<A>();

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<A> HashSet<A>(A head, params A[] tail) =>
        L.HashSet.createRange(head.Cons(tail));

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<A> toHashSet<A>(IEnumerable<A> items) =>
        items is HashSet<A> hs
            ? hs
            : L.HashSet.createRange(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<A> toHashSet<A>(Iterator<A> items) =>
        items is Iterator.IterHashSet<EqDefault<A>, A> xs
            ? new HashSet<A>(xs)
            : L.HashSet.createRange(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<A> toHashSet<A>(ReadOnlySpan<A> items) =>
        [..items];


    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqA, A> HashSet<EqA, A>() where EqA : Eq<A> =>
        L.HashSet.create<EqA, A>();

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqA, A> HashSet<EqA, A>(A head, params A[] tail) where EqA : Eq<A> =>
        L.HashSet.createRange<EqA, A>(head.Cons(tail));

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqA, A> toHashSet<EqA, A>(IEnumerable<A> items) where EqA : Eq<A> =>
        items is HashSet<EqA, A> hs
            ? hs
            : L.HashSet.createRange<EqA, A>(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqA, A> toHashSet<EqA, A>(Iterator<A> items) where EqA : Eq<A> =>
        items is Iterator.IterHashSet<EqA, A> xs
            ? new HashSet<EqA, A>(xs)
            : L.HashSet.createRange<EqA, A>(items);

    /// <summary>
    /// Create an immutable hash-set
    /// </summary>
    [Pure]
    public static HashSet<EqA, A> toHashSet<EqA, A>(ReadOnlySpan<A> items) where EqA : Eq<A> =>
        [..items];
}
