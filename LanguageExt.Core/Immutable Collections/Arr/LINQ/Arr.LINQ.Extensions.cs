using System;
using System.Numerics;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public readonly partial struct Arr<A>
{
    /// <summary>
    /// Projects each element of a range into a new form.
    /// </summary>
    [Pure]
    public Arr<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Filters a range of values based on a predicate.
    /// </summary>
    [Pure]
    public Arr<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    [Pure]
    public Arr<B> SelectMany<B>(Func<A, Arr<B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    [Pure]
    public Arr<B> SelectMany<B>(Func<A, K<Arr, B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    [Pure]
    public Arr<C> SelectMany<B, C>(Func<A, Arr<B>> bind, Func<A, B, C> project)
    {
        var ma     = this;
        var writer = ArrayWriterRef<C>.Init();

        foreach (var a in ma.ForwardIteratorRef<Arr, Arr.FoldState, A>())
        {
            var mb = bind(a);
            foreach (var b in mb.ForwardIteratorRef<Arr, Arr.FoldState, B>())
            {
                writer.Add(project(a, b));
            }
        }
        return writer.ToArr();
    }

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    [Pure]
    public Arr<C> SelectMany<B, C>(Func<A, K<Arr, B>> bind, Func<A, B, C> project)
    {
        var ma     = this;
        var writer = ArrayWriterRef<C>.Init();

        foreach (var a in ma.ForwardIteratorRef<Arr, Arr.FoldState, A>())
        {
            var mb = +bind(a);
            foreach (var b in mb.ForwardIteratorRef<Arr, Arr.FoldState, B>())
            {
                writer.Add(project(a, b));
            }
        }
        return writer.ToArr();
    }

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
    public Arr<A> Skip(int count) =>
        Skip((long)count);

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a range.
    /// </summary>
    [Pure]
    public Arr<A> Take(int count) =>
        Take((long)count);
}

public static partial class ArrExtensions
{
    extension<A>(Arr<A> ma)
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
