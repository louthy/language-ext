using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

#pragma warning disable CA2260
sealed class IterableConcat<A>(Seq<Iterable<A>> Items) : Iterable<A>
#pragma warning restore CA2260
{
    public Seq<Iterable<A>> Items { get; } = Items;

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public override int Count() =>
        Items.Fold(0, (s, iter) => s + iter.Count());

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public override IEnumerable<A> AsEnumerable()
    {
        foreach (var seq in Items)
        {
            foreach (var item in seq)
            {
                yield return item;
            }
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableConcat<A>(Items.Map(xs => xs.Reverse()).Rev());

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A> f)
    {
        foreach (var item in this)
        {
            f(item);
        }
        return default;
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A, int> f)
    {
        var ix = 0;
        foreach (var item in this)
        {
            f(item, ix);
            ix++;
        }
        return default;
    }

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Map(f)));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, int, B> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Map(f)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public override Iterable<B> Bind<B>(Func<A, Iterable<B>> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Bind(f)));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableConcat<A>(Items.Map(xs => xs.Filter(f)));

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(this);
}
