#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using System.ComponentModel;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// `Seq` module
/// </summary>
public partial class Seq
{
    [Obsolete("Use Combine or `+` operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<T> append<T>(Seq<T> lhs, Seq<T> rhs) =>
        lhs.Concat(rhs);

    [Obsolete("Use Combine or `+` operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<T> append<T>(Seq<T> x, Seq<Seq<T>> xs) =>
        head(xs).IsNone
            ? x
            : append(x, append((Seq<T>)xs.Head, xs.Skip(1)));

    [Obsolete("Use Combine or `+` operator instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<T> append<T>(params Seq<T>[] lists) =>
        lists.Length switch
        {
            0 => Seq<T>.Empty,
            1 => lists[0],
            _ => append(lists[0], toSeq(lists).Skip(1))
        };

    [Obsolete("Deprecated")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<Seq<A>> tailsr<A>(Seq<A> self) =>
        self.Match(
            () => Seq<Seq<A>>.Empty,
            xs => xs.Cons(tailsr(xs.Tail)));
}
