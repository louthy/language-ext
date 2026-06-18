using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public sealed partial class Iterable<A>
{
    /// <summary>
    /// Projects each element of a range into a new form.
    /// </summary>
    [Pure]
    public Iterable<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Filters a range of values based on a predicate.
    /// </summary>
    [Pure]
    public Iterable<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, Iterable<B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, K<Iterable, B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, Iterable<B>> bind, Func<A, B, C> project) =>
        new(iterator.SelectMany(x => bind(x).iterator, project));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, K<Iterable, B>> bind, Func<A, B, C> project) =>
        new(iterator.SelectMany(x => bind(x).As().iterator, project));

    /// <summary>
    /// Applies an accumulator function over a range.
    /// </summary>
    [Pure]
    public S Aggregate<S>(S state, Func<S, A, S> folder) =>
        this.ForwardIterator().Aggregate(state, folder);

    /// <summary>
    /// Determines whether any element of a range satisfies a condition.
    /// </summary>
    [Pure]
    public bool Any(Func<A, bool> predicate) =>
        this.ForwardIterator().Any(predicate);

    /// <summary>
    /// Determines whether all elements of a range satisfy a condition.
    /// </summary>
    [Pure]
    public bool All(Func<A, bool> predicate) =>
        this.ForwardIterator().ForAll(predicate);

    /// <summary>
    /// Returns the first element of a range.
    /// </summary>
    [Pure]
    public A First() =>
        this.ForwardIterator().First();

    /// <summary>
    /// Returns the first element of a range, or a default value if the range contains no elements.
    /// </summary>
    [Pure]
    public A? FirstOrDefault() =>
        this.ForwardIterator().FirstOrDefault();

    /// <summary>
    /// Bypasses a specified number of elements in a range and then returns the remaining elements.
    /// </summary>
    [Pure]
    public Iterable<A> Skip(int count) =>
        new(this.ForwardIterator().Skip(count));

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a range.
    /// </summary>
    [Pure]
    public Iterable<A> Take(int count) =>
        new(this.ForwardIterator().Take(count));

    /// <summary>
    /// Converts a range to an array.
    /// </summary>
    [Pure]
    public A[] ToArray() =>
        this.ToArr().ToArray();

    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> Order() =>
        this.ForwardIterator()
            .Order()
            .AsIterable();

    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> Order<OrdA>() 
        where OrdA : Ord<A> =>
        this.ForwardIterator()
            .Order<OrdA>()
            .AsIterable();


    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> Order(IComparer<A>? comparer) =>
        this.ForwardIterator()
            .Order(comparer)
            .AsIterable();


    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderBy<K>(Func<A, K> keySelector) => 
        this.ForwardIterator()
            .OrderBy(keySelector)
            .AsIterable();

    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderBy<OrdK, K>(Func<A, K> keySelector) 
        where OrdK : Ord<K> => 
        this.ForwardIterator()
            .OrderBy<OrdK, K>(keySelector)
            .AsIterable();

    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderBy<K>(Func<A, K> keySelector, IComparer<K>? comparer) =>
        this.ForwardIterator()
            .OrderBy(keySelector, comparer)
            .AsIterable();

    /// <summary>
    /// Yield items in ascending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderDescending<OrdA>() 
        where OrdA : Ord<A> =>
        this.ForwardIterator()
            .OrderDescending<OrdA>()
            .AsIterable();

    /// <summary>
    /// Yield items in descending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderDescending() =>
        this.ForwardIterator()
            .OrderDescending()
            .AsIterable();

    /// <summary>
    /// Yield items in descending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderDescending(IComparer<A>? comparer) =>
        OrderByDescending(Prelude.identity, comparer);

    /// <summary>
    /// Yield items in descending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderByDescending<K>(Func<A, K> keySelector) => 
        this.ForwardIterator()
            .OrderByDescending(keySelector)
            .AsIterable();

    /// <summary>
    /// Yield items in descending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderByDescending<OrdK, K>(Func<A, K> keySelector) 
        where OrdK : Ord<K> => 
        this.ForwardIterator()
            .OrderByDescending<OrdK, K>(keySelector)
            .AsIterable();

    /// <summary>
    /// Yield items in descending order 
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public Iterable<A> OrderByDescending<K>(Func<A, K> keySelector, IComparer<K>? comparer) => 
        this.ForwardIterator()
            .OrderByDescending(keySelector, comparer)
            .AsIterable();
}

public static partial class IterableExtensions
{
    extension<A>(Iterable<A> ma)
        where A : INumber<A>
    {
        /// <summary>
        /// Computes the sum of a range of numeric values.
        /// </summary>
        [Pure]
        public A Sum() =>
            ma.ForwardIterator().Sum();
    }
}
