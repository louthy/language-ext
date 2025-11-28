using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

#pragma warning disable CA2260
sealed class IterableEnumerable<A>(IEnumerable<A> runEnumerable) : Iterable<A>
#pragma warning restore CA2260
{
    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    public override int Count() =>
        runEnumerable.Count();

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public override IEnumerable<A> AsEnumerable() => 
        runEnumerable;

    [Pure]
    public override Iterable<A> Reverse() => 
        new IterableEnumerable<A>(runEnumerable.Reverse());

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A> f)
    {
        foreach (var x in runEnumerable)
        {
            f(x);
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
        foreach (var x in runEnumerable)
        {
            f(x, ix);
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
        new IterableEnumerable<B>(runEnumerable.Select(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, int, B> f) =>
        new IterableEnumerable<B>(
            runEnumerable.Zip(Naturals.AsEnumerable())
                         .Select(p => f(p.First, p.Second)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public override Iterable<B> Bind<B>(Func<A, Iterable<B>> f)
    {
        return new IterableEnumerable<B>(go(this, f));
        static IEnumerable<B> go(Iterable<A> ma, Func<A, Iterable<B>> bnd)
        {
            foreach (var a in ma.AsEnumerable())
            {
                foreach (var b in bnd(a).AsEnumerable())
                {
                    yield return b;
                }
            }
        }
    }

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public override Iterable<A> Filter(Func<A, bool> f)
    {
        return new IterableEnumerable<A>(Yield(this, f));
        static IEnumerable<A> Yield(Iterable<A> items, Func<A, bool> f)
        {
            foreach (var item in items.AsEnumerable())
            {
                if (f(item))
                {
                    yield return item;
                }
            }
        }
    }
    
    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(AsEnumerable());
}
