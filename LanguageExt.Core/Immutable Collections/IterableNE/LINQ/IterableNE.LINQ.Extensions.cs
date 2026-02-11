using System;
using System.Numerics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt;

public sealed partial class IterableNE<A>
{
    /// <summary>
    /// Projects each element of a range into a new form.
    /// </summary>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, B> f) =>
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
    public IterableNE<B> SelectMany<B>(Func<A, IterableNE<B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    [Pure]
    public IterableNE<C> SelectMany<B, C>(Func<A, IterableNE<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

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
}

public static partial class IterableNEExtensions
{
    extension<A>(IterableNE<A> ma)
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
