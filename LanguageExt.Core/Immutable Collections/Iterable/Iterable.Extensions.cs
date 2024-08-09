#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class IterableExtensions
{
    public static Iterable<A> As<A>(this K<Iterable, A> xs) =>
        (Iterable<A>)xs;
    
    public static Iterable<A> AsIterable<A>(this IEnumerable<A> xs) =>
        new IterableEnumerable<A>(xs);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Iterable<A> Flatten<A>(this Iterable<Iterable<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
    /// comprised of the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public static Iterable<B> Choose<A, B>(this Iterable<A> list, Func<A, Option<B>> selector) =>
        Iterable.choose(list, selector);

    [Pure]
    public static Iterable<T> Rev<T>(this Iterable<T> list) =>
        Iterable.rev(list);
}
