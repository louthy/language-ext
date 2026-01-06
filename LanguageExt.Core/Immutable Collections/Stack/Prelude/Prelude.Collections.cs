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
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<A> Stck<A>() =>
        L.Stck<A>.Empty;

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<A> Stck<A>(A top, params A[] rest) =>
        top.Top(Stck(rest.AsSpan()));

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<A> Stck<A>(ReadOnlySpan<A> items) =>
        [..items];

    /// <summary>
    /// Create an immutable stack
    /// </summary>
    [Pure]
    public static Stck<A> toStck<A>(IEnumerable<A> items) =>
        items is Stck<A> s
            ? s
            : L.Stck.createRange(items);
}
