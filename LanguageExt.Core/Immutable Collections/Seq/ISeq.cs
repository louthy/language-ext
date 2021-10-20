using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal class ISeqObsolete
    {
        public const string Message = "Please use Seq<A> directly.  ISeq<A> is less performant than Seq<A> and will go away eventually.  For implicit conversions of ISeq<A> to ISeq<B> you can use seqA.Cast<B>() for an efficient mapping between compatible bound types";
    }

    /// <summary>
    /// Cons sequence
    /// Represents a sequence of values in a similar way to IEnumerable, but without the
    /// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
    /// </summary>
    [Obsolete(ISeqObsolete.Message)]
    public interface ISeq<A> : IEquatable<ISeq<A>>, IComparable<ISeq<A>>, IEnumerable<A>
    {
        /// <summary>
        /// Head of the sequence
        /// </summary>
        A Head { get; }

        /// <summary>
        /// Head of the sequence
        /// </summary>
        Option<A> HeadOrNone();

        /// <summary>
        /// Head of the sequence
        /// </summary>
        Validation<Fail, A> HeadOrInvalid<Fail>(Fail fail);

        /// <summary>
        /// Head of the sequence
        /// </summary>
        Validation<MonoidFail, Fail, A> HeadOrInvalid<MonoidFail, Fail>(Fail fail) where MonoidFail : struct, Monoid<Fail>, Eq<Fail>;

        /// <summary>
        /// Head of the sequence
        /// </summary>
        Either<L, A> HeadOrLeft<L>(L left);

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        Seq<A> Tail { get; }

        /// <summary>
        /// True if this cons node is the Empty node
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        int Count { get; }

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, Seq<A>, B> Tail);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<A, Seq<A>, B> Tail);

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Seq">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<Seq<A>, B> Seq);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<Seq<A>, B> Tail);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        Seq<B> Map<B>(Func<A, B> f);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        Seq<B> Select<B>(Func<A, B> f);

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        Seq<A> Filter(Func<A, bool> f);

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        Seq<A> Where(Func<A, bool> f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        Seq<B> Bind<B>(Func<A, Seq<B>> f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="bind">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project);

        /// <summary>
        /// Fold the sequence from the first item to the last
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        S Fold<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Fold the sequence from the last item to the first. 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        S FoldBack<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.</returns>
        bool Exists(Func<A, bool> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.</returns>
        bool ForAll(Func<A, bool> f);

        /// <summary>
        /// Skip count items
        /// </summary>
        Seq<A> Skip(int count);

        /// <summary>
        /// Take count items
        /// </summary>
        Seq<A> Take(int count);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        Seq<A> TakeWhile(Func<A, bool> pred);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't.  An index value is 
        /// also provided to the predicate function.
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        Seq<A> TakeWhile(Func<A, int, bool> pred);
    }
}
