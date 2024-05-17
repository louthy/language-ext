#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class EnumerableMExtensions
{
    public static EnumerableM<A> As<A>(this K<EnumerableM, A> xs) =>
        (EnumerableM<A>)xs;
    
    public static EnumerableM<A> AsEnumerableM<A>(this IEnumerable<A> xs) =>
        new (xs);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static EnumerableM<A> Flatten<A>(this EnumerableM<EnumerableM<A>> ma) =>
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
    public static EnumerableM<B> Choose<A, B>(this EnumerableM<A> list, Func<A, Option<B>> selector) =>
        EnumerableM.choose(list, selector);

    [Pure]
    public static EnumerableM<T> Rev<T>(this EnumerableM<T> list) =>
        EnumerableM.rev(list);
}
