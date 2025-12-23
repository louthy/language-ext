#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IterableNEExtensions
{
    [Pure]
    public static IterableNE<A> As<A>(this K<IterableNE, A> xs) =>
        (IterableNE<A>)xs;

    [Pure]
    public static Option<IterableNE<A>> AsIterableNE<A>(this IEnumerable<A> xs) =>
        IterableNE.createRange(xs);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Option<IterableNE<A>> Flatten<A>(this IterableNE<IterableNE<A>> ma) =>
        ma.Bind(identity);

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(IterableNE<A> list)
    {
        /// <summary>
        /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
        /// comprised of the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered sequence</returns>
        [Pure]
        public Iterable<B> Choose<B>(Func<A, Option<B>> selector) =>
            IterableNE.choose(list, selector);
    }
}
