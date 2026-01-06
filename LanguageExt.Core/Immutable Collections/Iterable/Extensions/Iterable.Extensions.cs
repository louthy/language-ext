#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IterableExtensions
{
    extension<A>(IEnumerable<A> xs)
    {
        public Iterable<A> AsIterable() =>
            new IterableEnumerable<A>(IO.pure(xs));
    }
    
    extension<A>(IAsyncEnumerable<A> xs)
    {
        public Iterable<A> AsIterableAsync() =>
            new IterableAsyncEnumerable<A>(IO.pure(xs));
    }

    extension<A>(Iterable<Iterable<A>> ma)
    {
        public Iterable<A> Flatten() =>
            ma.Bind(identity);
    }

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<Iterable, A> list)
    {
        /// <summary>
        /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
        /// composed of the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <param name="f">Selector function</param>
        /// <returns>Mapped and filtered sequence</returns>
        [Pure]
        public Iterable<B> Choose<B>(Func<A, Option<B>> f) =>
            Iterable.choose(+list, f);

        [Pure]
        public Iterable<A> Rev() =>
            Iterable.rev(+list);

        public Iterable<A> As() =>
            (Iterable<A>)list;
    }

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<Iterable, A> list)
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
            list.As().FoldIO(Monoid.combine, A.Empty);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public A FoldWhile(Func<(A State, A Value), bool> predicate) =>
            list.As().FoldWhileIO(predicate).Run();

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public IO<A> FoldWhileIO(Func<(A State, A Value), bool> predicate) =>
            list.As().FoldWhileIO(Monoid.combine, predicate, A.Empty);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public A FoldUntil(Func<(A State, A Value), bool> predicate) =>
            list.As().FoldUntilIO(predicate).Run();

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        public IO<A> FoldUntilIO(Func<(A State, A Value), bool> predicate) =>
            list.As().FoldUntilIO(Monoid.combine, predicate, A.Empty);
    }
}
