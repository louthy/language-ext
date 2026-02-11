using System;
using System.Numerics;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt;

public static partial class RangeExtensions
{
    extension<A>(Range<A> ma)
    {
        /// <summary>
        /// Projects each element of a range into a new form.
        /// </summary>
        [Pure]
        public Iterator<B> Select<B>(Func<A, B> f) =>
            ma.ForwardIterator().Map(f);

        /// <summary>
        /// Projects each element of a range into a new form.
        /// </summary>
        [Pure]
        public Iterator<B> Map<B>(Func<A, B> f) =>
            ma.ForwardIterator().Map(f);

        /// <summary>
        /// Filters a range of values based on a predicate.
        /// </summary>
        [Pure]
        public Iterator<A> Where(Func<A, bool> f) =>
            ma.ForwardIterator().Filter(f);

        /// <summary>
        /// Filters a range of values based on a predicate.
        /// </summary>
        [Pure]
        public Iterator<A> Filter(Func<A, bool> f) =>
            ma.ForwardIterator().Filter(f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        [Pure]
        public Iterator<B> SelectMany<B>(Func<A, Iterator<B>> f) =>
            ma.ForwardIterator().Bind(f);

        /// <summary>
        /// Monadic bind
        /// </summary>
        [Pure]
        public Iterator<B> Bind<B>(Func<A, Iterator<B>> f) =>
            ma.ForwardIterator().Bind(f);

        /// <summary>
        /// Monadic bind and project
        /// </summary>
        [Pure]
        public Iterator<C> SelectMany<B, C>(Func<A, Iterator<B>> bind, Func<A, B, C> project) =>
            ma.ForwardIterator().SelectMany(bind, project);

        /// <summary>
        /// Applies an accumulator function over a range.
        /// </summary>
        [Pure]
        public S Aggregate<S>(S state, Func<S, A, S> folder) =>
            ma.ForwardIterator().Aggregate(state, folder);

        /// <summary>
        /// Returns the number of elements in a range.
        /// </summary>
        [Pure]
        public long Count() =>
            ma.ForwardIterator().Count();

        /// <summary>
        /// Determines whether any element of a range satisfies a condition.
        /// </summary>
        [Pure]
        public bool Any(Func<A, bool> predicate) =>
            ma.ForwardIterator().Any(predicate);

        /// <summary>
        /// Determines whether all elements of a range satisfy a condition.
        /// </summary>
        [Pure]
        public bool All(Func<A, bool> predicate) =>
            ma.ForwardIterator().ForAll(predicate);

        /// <summary>
        /// Returns the first element of a range.
        /// </summary>
        [Pure]
        public A First() =>
            ma.ForwardIterator().First();

        /// <summary>
        /// Returns the first element of a range, or a default value if the range contains no elements.
        /// </summary>
        [Pure]
        public A? FirstOrDefault() =>
            ma.ForwardIterator().FirstOrDefault();

        /// <summary>
        /// Bypasses a specified number of elements in a range and then returns the remaining elements.
        /// </summary>
        [Pure]
        public Iterator<A> Skip(int count) =>
            ma.ForwardIterator().Skip(count);

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a range.
        /// </summary>
        [Pure]
        public Iterator<A> Take(int count) =>
            ma.ForwardIterator().Take(count);

        /// <summary>
        /// Converts a range to an array.
        /// </summary>
        [Pure]
        public A[] ToArray() =>
            ma.ToArr().ToArray();
    }

    extension<A>(Range<A> ma)
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
