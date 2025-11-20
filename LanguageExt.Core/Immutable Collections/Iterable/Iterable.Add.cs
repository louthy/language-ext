using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

#pragma warning disable CA2260
sealed class IterableAdd<A>(SeqStrict<A> Prefix, Iterable<A> Source, SeqStrict<A> Postfix) : Iterable<A>
#pragma warning restore CA2260
{
    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public override int Count() =>
        Prefix.Count + Source.Count() + Postfix.Count;

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public override Iterable<A> Add(A item) =>
        new IterableAdd<A>(Prefix, Source, (SeqStrict<A>)Postfix.Add(item));

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public override Iterable<A> Cons(A item) =>
        new IterableAdd<A>((SeqStrict<A>)Prefix.Cons(item), Source, Postfix);

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public override IEnumerable<A> AsEnumerable()
    {
        foreach (var x in Prefix)
        {
            yield return x;
        }
        foreach (var x in Source)
        {
            yield return x;
        }
        foreach (var x in Postfix)
        {
            yield return x;
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableAdd<A>(Postfix.Rev(), Source.Rev(), Prefix.Rev());

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public override Unit Iter(Action<A> f)
    {
        foreach (var a in Prefix)
        {
            f(a);
        }
        foreach (var a in Source)
        {
            f(a);
        }
        foreach (var a in Postfix)
        {
            f(a);
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
        foreach (var a in Prefix)
        {
            f(a, ix);
            ix++;
        }
        foreach (var a in Source)
        {
            f(a, ix);
            ix++;
        }
        foreach (var a in Postfix)
        {
            f(a, ix);
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
    public override Iterable<B> Map<B>(Func<A, B> f)
    {
        return new IterableEnumerable<B>(Yield());
        IEnumerable<B> Yield()
        {
            foreach (var a in Prefix)
            {
                yield return f(a);
            }
            foreach (var a in Source)
            {
                yield return f(a);
            }
            foreach (var a in Postfix)
            {
                yield return f(a);
            }
        }
    }

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public override Iterable<B> Map<B>(Func<A, int, B> f)
    {
        return new IterableEnumerable<B>(Yield());
        IEnumerable<B> Yield()
        {
            var ix = 0;
            foreach (var a in Prefix)
            {
                yield return f(a, ix);
                ix++;
            }
            foreach (var a in Source)
            {
                yield return f(a, ix);
                ix++;
            }
            foreach (var a in Postfix)
            {
                yield return f(a, ix);
                ix++;
            }
        }
    }
    
    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public override Iterable<B> Bind<B>(Func<A, Iterable<B>> f)
    {
        return new IterableEnumerable<B>(Yield());
        IEnumerable<B> Yield()
        {
            foreach (var a in Prefix)
            {
                foreach (var b in f(a))
                {
                    yield return b;
                }
            }
            foreach (var a in Source)
            {
                foreach (var b in f(a))
                {
                    yield return b;
                }
            }
            foreach (var a in Postfix)
            {
                foreach (var b in f(a))
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
        return new IterableEnumerable<A>(Yield());
        IEnumerable<A> Yield()
        {
            foreach (var a in Prefix)
            {
                if (f(a)) yield return a;
            }
            foreach (var a in Source)
            {
                if (f(a)) yield return a;
            }
            foreach (var a in Postfix)
            {
                if (f(a)) yield return a;
            }
        }
    }

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(AsEnumerable());
}
