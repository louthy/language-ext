#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IterableExtensions
{
    public static Iterable<A> As<A>(this K<Iterable, A> xs) =>
        (Iterable<A>)xs;
    
    public static Iterable<A> AsIterable<A>(this IEnumerable<A> xs) =>
        new IterableEnumerable<A>(IO.pure(xs));
    
    public static Iterable<A> AsIterable<A>(this IAsyncEnumerable<A> xs) =>
        new IterableAsyncEnumerable<A>(IO.pure(xs));

    public static Iterable<A> Flatten<A>(this Iterable<Iterable<A>> ma) =>
        ma.Bind(identity);

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(Iterable<A> list)
    {
        /// <summary>
        /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
        /// comprised of the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered sequence</returns>
        [Pure]
        public Iterable<B> Choose<B>(Func<A, Option<B>> selector) =>
            Iterable.choose(list, selector);

        [Pure]
        public Iterable<A> Rev() =>
            Iterable.rev(list);
    }

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(Iterable<A> list)
        where A : Monoid<A>
    {
        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public A Fold() =>
            list.FoldIO().Run();

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public IO<A> FoldIO() =>
            list.FoldMapIO(identity); 

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public A FoldWhile(Func<(A State, A Value), bool> predicate) =>
            list.FoldWhileIO(predicate).Run();

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public IO<A> FoldWhileIO(Func<(A State, A Value), bool> predicate) =>
            list.FoldMapWhileIO(identity, predicate);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public A FoldUntil(Func<(A State, A Value), bool> predicate) =>
            list.FoldUntilIO(predicate).Run();

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public IO<A> FoldUntilIO(Func<(A State, A Value), bool> predicate) =>
            list.FoldMapUntilIO(identity, predicate);
    }
}
