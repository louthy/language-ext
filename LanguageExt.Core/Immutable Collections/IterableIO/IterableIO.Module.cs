using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Module for IterableIO
/// </summary>
public partial class IterableIO
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IterableIO<A> flatten<A>(K<IterableIO, IterableIO<A>> items) =>
        new(items.As().iterator.Map(i => i.iterator).Flatten());

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    public static IterableIO<A> empty<A>() =>
        IterableIO<A>.Empty;

    /// <summary>
    /// Create a sequence with a single item
    /// </summary>
    [Pure]
    public static IterableIO<A> singleton<A>(A value) =>
        new(IteratorIO.singleton(value));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableIO<A> create<A>(params A[] items) =>
        items.Length == 0
            ? IterableIO<A>.Empty
            : IterableIO<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableIO<A> create<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 
            ? IterableIO<A>.Empty 
            : IterableIO<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableIO<A> createRange<A>(IEnumerable<A> items) =>
        new (items.AsIteratorIO());

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static IterableIO<A> createRange<A>(IAsyncEnumerable<A> items) =>
        new (items.AsIteratorIO());
    
    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static IterableIO<A> generate<A>(Func<long, A> generator) =>
        Range(0L, long.MaxValue).Map(generator).AsIterableIO();
    
    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static IterableIO<A> generate<A>(long count, Func<long, A> generator) =>
        Range(0L, count).Map(generator).AsIterableIO();

    /// <summary>
    /// Generates a sequence that contains one value repeated many times.
    /// </summary>
    [Pure]
    public static IterableIO<A> repeat<A>(A item, long count) =>
        Range(0L, count).Map(_ => item).AsIterableIO();

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `None` if the sequence is empty
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    public static IO<Option<A>> head<A>(K<IterableIO, A> items) =>
        items.As().Head;

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `None` if the sequence is empty
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    public static K<M, A> head<M, A>(K<IterableIO, A> items) 
        where M : MonadIO<M>, Alternative<M> =>
        items.As().HeadM<M>();

    /// <summary>
    /// Consume the first item of the sequence, returning the tail of the sequence. 
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>The tail items</returns>
    [Pure]
    public static IO<IterableIO<A>> tail<A>(K<IterableIO, A>items) =>
        items.As().Tail;

    /// <summary>
    /// Applies the given function `f` to each element of the sequence. Returns the sequence 
    /// of results for each element where the result is `Some(f(x))`.
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <param name="f">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public static IterableIO<B> choose<A, B>(K<IterableIO, A> items, Func<A, Option<B>> f) =>
        items.As().Choose(f);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    public static IterableIO<A> rev<A>(K<IterableIO, A> items) =>
        items.As().Reverse();

    /// <summary>
    /// Joins two sequences together either into a single sequence using the join 
    /// function provided
    /// </summary>
    /// <param name="first">First sequence to join</param>
    /// <param name="second">Second sequence to join</param>
    /// <param name="join">Join function</param>
    /// <returns>Joined sequence</returns>
    [Pure]
    public static IterableIO<C> zip<A, B, C>(K<IterableIO, A> first, K<IterableIO, B> second, Func<A, B, C> join) =>
        first.As().Zip(+second, join);

    /// <summary>
    /// Joins two sequences together either into a sequence of tuples
    /// </summary>
    /// <param name="first">First sequence to join</param>
    /// <param name="second">Second sequence to join</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    public static IterableIO<(A First, B Second)> zip<A, B>(K<IterableIO, A> first, K<IterableIO, B> second) =>
        first.As().Zip(+second);

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static IterableIO<A> distinct<A>(K<IterableIO, A> items) =>
        items.As().Distinct();

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static IterableIO<A> distinct<EqA, A>(K<IterableIO, A> items) where EqA : Eq<A> =>
        items.As().Distinct<EqA>();

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public static IterableIO<A> skip<A>(K<IterableIO, A> items, int amount) =>
        items.As().Skip(amount);

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public static IterableIO<A> skipWhile<A>(K<IterableIO, A> items, Func<A, bool> predicate) =>
        items.As().SkipWhile(predicate);

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public static IterableIO<A> skipUntil<A>(K<IterableIO, A> items, Func<A, bool> predicate) =>
        items.As().SkipUntil(predicate);

    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public static IterableIO<A> take<A>(K<IterableIO, A> items, int amount) =>
        items.As().Take(amount);

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public static IterableIO<A> takeWhile<A>(K<IterableIO, A> items, Func<A, bool> predicate) =>
        items.As().TakeWhile(predicate);

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public static IterableIO<A> takeUntil<A>(K<IterableIO, A> items, Func<A, bool> predicate) =>
        items.As().TakeUntil(predicate);
    
    /// <summary>
    /// Invokes an action for each item in the sequence
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="items">Enumerable to iterate</param>
    /// <param name="f">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static IO<Unit> iter<A>(K<IterableIO, A> items, Action<A> f) =>
        items.IterIO(f);

    /// <summary>
    /// Invokes an action for each item in the sequence
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="items">Enumerable to iterate</param>
    /// <param name="f">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static IO<Unit> iter<A>(K<IterableIO, A> items, Action<long, A> f) =>
        items.IterIO(f);
    
    /// <summary>
    /// Generate a new sequence from an initial state value and an 'unfolding' function.
    /// The unfold function generates the items in the resulting sequence until `None` is
    /// returned.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="unfolder">Unfold function</param>
    /// <returns>Unfolded sequence</returns>
    [Pure]
    public static IterableIO<S> unfold<S>(S state, Func<S, Option<S>> unfolder)
    {
        return IteratorIO.cons(state, () => go(state)).AsIterable();

        // ReSharper disable once VariableHidesOuterVariable
        IteratorIO<S> go(S state) =>
            unfolder(state) switch
            {
                { IsSome: true, Case: S ns } => IteratorIO.cons(ns, () => go(ns)),
                _                            => IteratorIO.empty<S>()
            };
    }

    /// <summary>
    /// Generate a new sequence from an initial state value and an 'unfolding' function.  An aggregate
    /// state value is threaded through separately to the yielded value. The unfold function generates
    /// the items in the resulting sequence until `None` is returned.
    /// </summary>
    /// <typeparam name="A">Element item type</typeparam>
    /// <typeparam name="S">State value type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="unfolder">Unfold function</param>
    /// <returns>Unfolded sequence</returns>
    [Pure]
    public static IterableIO<A> unfold<S, A>(S state, Func<S, Option<(A, S)>> unfolder)
    {
        return go(state).AsIterable();

        // ReSharper disable once VariableHidesOuterVariable
        IteratorIO<A> go(S state) =>
            unfolder(state) switch
            {
                { IsSome: true, Value: var n } => IteratorIO.cons(n.Item1, () => go(n.Item2)),
                _                              => IteratorIO.empty<A>()
            };
    }

}
