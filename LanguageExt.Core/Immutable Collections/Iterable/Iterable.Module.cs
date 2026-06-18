using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

namespace LanguageExt;

/// <summary>
/// Module for Iterable
/// </summary>
public partial class Iterable
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Iterable<A> flatten<A>(K<Iterable, Iterable<A>> ma) =>
        new (ma.As().iterator.Map(i => i.iterator).Flatten());

    /// <summary>
    /// Create an empty sequence
    /// </summary>
    [Pure]
    public static Iterable<A> empty<A>() =>
        Iterable<A>.Empty;

    /// <summary>
    /// Create a sequence with a single item
    /// </summary>
    [Pure]
    public static Iterable<A> singleton<A>(A value) =>
        new (Iterator.singleton(value));

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(ReadOnlySpan<A> items) =>
        items.Length == 0 
            ? Iterable<A>.Empty 
            : Iterable<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(params A[] items) =>
        items.Length == 0
            ? Iterable<A>.Empty
            : Iterable<A>.FromSpan(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<T, A>(K<T, A> items)
        where T : IterableK<T> =>
        items.ForwardIterator().AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Arr<A> items) =>
        create<Arr, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<K, V>(HashMap<K, V> items) =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<EqK, K, V>(HashMap<EqK, K, V> items) 
        where EqK : Eq<K> =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(HashSet<A> items) =>
        create<HashSet, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<EqA, A>(HashSet<EqA, A> items) 
        where EqA : Eq<A> =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(IterableNE<A> items) =>
        items.Head.Cons(items.Tail).AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Iterator<A> items) =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Lst<A> items) =>
        create<Lst, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<K, V>(Map<K, V> items) =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<OrdK, K, V>(Map<OrdK, K, V> items) 
        where OrdK : Ord<K> =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Que<A> items) =>
        create<Que, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Seq<A> items) =>
        create<Seq, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Set<A> items) =>
        create<Set, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<OrdA, A>(Set<OrdA, A> items) 
        where OrdA : Ord<A> =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<A> create<A>(Stck<A> items) =>
        create<Stck, A>(items);

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<K, V>(TrackingHashMap<K, V> items) =>
        items.AsIterable();

    /// <summary>
    /// Create a sequence from an initial set of items
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns>sequence</returns>
    [Pure]
    public static Iterable<(K Key, V Value)> create<EqK, K, V>(TrackingHashMap<EqK, K, V> items) 
        where EqK : Eq<K> =>
        items.AsIterable();

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Iterable<A> generate<A>(Func<long, A> generator) =>
        generate(long.MaxValue, generator);

    /// <summary>
    /// Generates a sequence of A using the provided delegate to initialise
    /// each item.
    /// </summary>
    [Pure]
    public static Iterable<A> generate<A>(long count, Func<long, A> generator) =>
        new(Range(0L, count).Map(generator));

    /// <summary>
    /// Generates a sequence that contains one value repeated many times.
    /// </summary>
    [Pure]
    public static Iterable<A> repeat<A>(A item, long count) =>
        new(Range(0L, count).Map(_ => item));

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `None` if the sequence is empty
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    public static Option<A> head<A>(K<Iterable, A> items) =>
        items.As().Head;

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `None` if the sequence is empty
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>Optional head item</returns>
    [Pure]
    public static K<M, A> head<M, A>(K<Iterable, A> items) 
        where M : Alternative<M> =>
        items.As().HeadM<M>();

    /// <summary>
    /// Consume the first item of the sequence, returning the tail of the sequence. 
    /// </summary>
    /// <param name="items">sequence</param>
    /// <returns>The tail items</returns>
    [Pure]
    public static Iterable<A> tail<A>(K<Iterable, A> items) =>
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
    public static Iterable<B> choose<A, B>(K<Iterable, A> items, Func<A, Option<B>> f) =>
        items.As().Choose(f);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    public static Iterable<A> rev<A>(K<Iterable, A> items) =>
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
    public static Iterable<C> zip<A, B, C>(K<Iterable, A> first, K<Iterable, B> second, Func<A, B, C> join) =>
        first.As().Zip(+second, join);

    /// <summary>
    /// Joins two sequences together either into a sequence of tuples
    /// </summary>
    /// <param name="first">First sequence to join</param>
    /// <param name="second">Second sequence to join</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    public static Iterable<(A First, B Second)> zip<A, B>(K<Iterable, A> first, K<Iterable, B> second) =>
        first.As().Zip(+second);

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Iterable<A> distinct<A>(K<Iterable, A> items) =>
        items.As().Distinct();

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="items">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Iterable<A> distinct<EqA, A>(K<Iterable, A> items) where EqA : Eq<A> =>
        items.As().Distinct<EqA>();

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public static Iterable<A> skip<A>(K<Iterable, A> items, int amount) =>
        items.As().Skip(amount);

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public static Iterable<A> skipWhile<A>(K<Iterable, A> items, Func<A, bool> predicate) =>
        items.As().SkipWhile(predicate);

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public static Iterable<A> skipUntil<A>(K<Iterable, A> items, Func<A, bool> predicate) =>
        items.As().SkipUntil(predicate);

    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public static Iterable<A> take<A>(K<Iterable, A> items, int amount) =>
        items.As().Take(amount);

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public static Iterable<A> takeWhile<A>(K<Iterable, A> items, Func<A, bool> predicate) =>
        items.As().TakeWhile(predicate);

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public static Iterable<A> takeUntil<A>(K<Iterable, A> items, Func<A, bool> predicate) =>
        items.As().TakeUntil(predicate);

    /// <summary>
    /// Invokes an action for each item in the sequence
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="items">Enumerable to iterate</param>
    /// <param name="f">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit iter<A>(K<Iterable, A> items, Action<A> f) =>
        items.Iter(f);

    /// <summary>
    /// Invokes an action for each item in the sequence
    /// </summary>
    /// <typeparam name="A">Element type</typeparam>
    /// <param name="items">Enumerable to iterate</param>
    /// <param name="f">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit iter<A>(K<Iterable, A> items, Action<long, A> f) =>
        items.Iter(f);

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
    public static Iterable<S> unfold<S>(S state, Func<S, Option<S>> unfolder)
    {
        return Iterator.cons(state, () => go(state)).AsIterable();

        // ReSharper disable once VariableHidesOuterVariable
        Iterator<S> go(S state) =>
            unfolder(state) switch
            {
                { IsSome: true, Case: S ns } => Iterator.cons(ns, () => go(ns)),
                _                            => Iterator.empty<S>()
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
    public static Iterable<A> unfold<S, A>(S state, Func<S, Option<(A, S)>> unfolder)
    {
        return go(state).AsIterable();

        // ReSharper disable once VariableHidesOuterVariable
        Iterator<A> go(S state) =>
            unfolder(state) switch
            {
                { IsSome: true, Value: var n } => Iterator.cons(n.Item1, () => go(n.Item2)),
                _                              => Iterator.empty<A>()
            };
    }
}
