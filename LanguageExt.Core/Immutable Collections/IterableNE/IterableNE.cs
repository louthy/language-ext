using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LanguageExt;

/// <summary>
/// Non-empty lazy-sequence
/// </summary>
/// <remarks>
/// This always has a Head value and a Tail of length 0 to `n`.   
/// </remarks>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
[CollectionBuilder(typeof(IterableNE), nameof(IterableNE.createRangeUnsafe))]
public sealed record IterableNE<A>(A Head, Iterable<A> Tail) :
    IEnumerable<A>,
    Semigroup<IterableNE<A>>,
    IComparable<IterableNE<A>>,
    IComparisonOperators<IterableNE<A>, IterableNE<A>, bool>,
    IAdditionOperators<IterableNE<A>, IterableNE<A>, IterableNE<A>>,
    K<IterableNE, A>
{
    int? hashCode;

    internal static IterableNE<A> FromSpan(ReadOnlySpan<A> ma)
    {
        if(ma.IsEmpty) throw new ArgumentException("Cannot create an IterableNE from an empty span");
        return new IterableNE<A>(ma[0], Iterable<A>.FromSpan(ma.Slice(1)));
    }

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public int Count() => 
        1 + Tail.Count();

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
        yield return Head;
        foreach (var item in Tail.AsEnumerable())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() =>
        AsEnumerable().AsIterable();

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Add(A item) =>
         new (Head, Tail.Add(item));

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Cons(A item) =>
        new (item, Head.Cons(Tail));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A> f)
    {
        f(Head);
        return Tail.Iter(f);
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A, int> f)
    {
        f(Head, 0);
        var ix = 0;
        foreach (var x in Tail)
        {
            f(x, ++ix);
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
    public IterableNE<B> Map<B>(Func<A, B> f) =>
        new (f(Head), Tail.Map(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Map<B>(Func<A, int, B> f)
    {
        return new(f(Head, 0), go().AsIterable());

        IEnumerable<B> go()
        {
            var ix = 0;
            foreach (var x in Tail)
            {
                yield return f(x, ++ix);
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
    public IterableNE<B> Bind<B>(Func<A, IterableNE<B>> f)
    {
        return IterableNE.createRangeUnsafe(go());
        IEnumerable<B> go()
        {
            foreach (var b in f(Head))
            {
                yield return b;
            }

            foreach (var a in Tail)
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
    public Option<IterableNE<A>> Filter(Func<A, bool> f)
    {
        return IterableNE.createRange(go());

        IEnumerable<A> go()
        {
            if(f(Head)) yield return Head;
            foreach (var a in Tail)
            {
                if(f(a)) yield return a;
            }
        }
    }

    [Pure]
    public S FoldWhile<S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state)
    {
        foreach (var x in AsEnumerable())
        {
            if (!predicate((state, x))) return state;
            state = f(x)(state);
        }
        return state;
    }

    [Pure]
    public S FoldWhile<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S state)
    {
        foreach (var x in AsEnumerable())
        {
            if (!predicate((state, x))) return state;
            state = f(state, x);
        }
        return state;
    }

    [Pure]
    public S FoldBackWhile<S>(
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S state)
    {
        foreach (var x in AsEnumerable().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }

    [Pure]
    public S FoldBackWhile<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S state)
    {
        foreach (var x in AsEnumerable().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state, x);
        }
        return state;
    }

    /// <summary>
    /// Semigroup combine two iterables (concatenate)
    /// </summary>
    [Pure]
    public IterableNE<A> Combine(IterableNE<A> y) =>
        new(Head, Tail + y.Head.Cons(y.Tail));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(IEnumerable<A> items) =>
        new(Head, Tail.Concat(items));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(Iterable<A> items) =>
        new(Head, Tail + items);

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(IterableNE<A> items) =>
        new(Head, Tail + items.Head.Cons(items.Tail));

    [Pure]
    public IterableNE<A> Reverse() =>
        IterableNE.createRangeUnsafe(AsEnumerable().Reverse());

    /// <summary>
    /// Applies a function to each element of the sequence, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the sequence. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public IterableNE<S> Scan<S>(S state, Func<S, A, S> folder) =>
        IterableNE.createRangeUnsafe(AsIterable().Scan(state, folder));

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the sequence. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public IterableNE<S> ScanBack<S>(S state, Func<S, A, S> folder) =>
        IterableNE.createRangeUnsafe(AsIterable().ScanBack(state, folder));
    
    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public IterableNE<A> Distinct<EqA>()
        where EqA : Eq<A>
    {
        return IterableNE.createRangeUnsafe(Yield());
        
        IEnumerable<A> Yield()
        {
            HashSet<EqA, A> set = [];
            foreach (var x in AsEnumerable())
            {
                if(set.Contains(x)) continue;
                set = set.Add(x);
                yield return x;
            }
        }
    }

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public IterableNE<A> Distinct() =>
        Distinct<EqDefault<A>>();
    
    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> Bind<B>(Func<A, K<IterableNE, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        true;

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public IterableNE<A> Intersperse(A value)
    {
        return IterableNE.createRangeUnsafe(Yield());
        IEnumerable<A> Yield()
        {
            {
                var isFirst = true;
                foreach(var item in AsEnumerable())
                {
                    if (!isFirst)
                    {
                        yield return value;
                    }

                    yield return item;
                    isFirst = false;
                }
            }
        }
    }

    [Pure]
    public int CompareTo(object? obj) =>
        obj is IterableNE<A> rhs
            ? CompareTo(rhs)
            : 1;

    [Pure]
    public int CompareTo(IterableNE<A>? other) =>
        CompareTo<OrdDefault<A>>(other);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(IterableNE<A>? rhs) 
        where OrdA : Ord<A>
    {
        if (rhs is null) return 1;        
        using var iterA = GetEnumerator();
        using var iterB = rhs.AsEnumerable().GetEnumerator();
        while (iterA.MoveNext())
        {
            if (iterB.MoveNext())
            {
                var cmp = OrdA.Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            else
            {
                return 1;
            }
        }

        if (iterB.MoveNext())
        {
            return -1;
        }

        return 0;
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

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(AsEnumerable(), separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(AsEnumerable(), separator);

    /// <summary>
    /// Tail of the iterable
    /// </summary>
    public Option<IterableNE<A>> TailNE =>
        IterableNE.createRange(Tail);
    
    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    public Option<IterableNE<A>> Skip(int amount) =>
        IterableNE.createRange(AsEnumerable().Skip(amount));

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    public Option<IterableNE<A>> Take(int amount) =>
        IterableNE.createRange(AsEnumerable().Take(amount));

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Option<IterableNE<A>> TakeWhile(Func<A, bool> pred)
    {
        return IterableNE.createRange(Yield(AsEnumerable(), pred));
        IEnumerable<A> Yield(IEnumerable<A> xs, Func<A, bool> f)
        {
            foreach (var x in xs)
            {
                if (!f(x)) break;
                yield return x;
            }
        }
    }

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't.  An index value is 
    /// also provided to the predicate function.
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Option<IterableNE<A>> TakeWhile(Func<A, int, bool> pred)
    {
        return IterableNE.createRange(Yield(AsEnumerable(), pred));
        IEnumerable<A> Yield(IEnumerable<A> xs, Func<A, int, bool> f)
        {
            var i = 0;
            foreach (var x in xs)
            {
                if (!f(x, i)) break;
                yield return x;
                i++;
            }
        }
    }

    [Pure]
    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    public (Iterable<A> First, Iterable<A> Second) Partition(Func<A, bool> predicate)
    {
        var f = List<A>();
        var s = List<A>();
        foreach (var item in AsEnumerable())
        {
            if (predicate(item))
            {
                f = f.Add(item);
            }
            else
            {
                s = s.Add(item);
            }
        }
        return (new IterableEnumerable<A>(f), new IterableEnumerable<A>(s));
    }

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<(A First, B Second)> Zip<B>(IterableNE<B> rhs) =>
        IterableNE.createRangeUnsafe(AsEnumerable().Zip(rhs.AsEnumerable()));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<C> Zip<B, C>(IterableNE<B> rhs, Func<A, B, C> zipper) =>
        IterableNE.createRangeUnsafe(AsEnumerable().Zip(rhs.AsEnumerable(), zipper));

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(IterableNE<A> x, IterableNE<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> operator |(IterableNE<A> x, K<IterableNE, A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IterableNE<A> operator |(K<IterableNE, A> x, IterableNE<A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) <= 0;

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, int, B> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> SelectMany<B>(Func<A, IterableNE<B>> bind) =>
        Bind(bind);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<C> SelectMany<B, C>(Func<A, IterableNE<B>> bind, Func<A, B, C> project) =>
        Bind(x  => bind(x).Map(y => project(x, y)));

    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Get the hash code for all the items in the sequence, or 0 if empty
    /// </summary>
    /// <returns></returns>
    [Pure]
    public override int GetHashCode() =>
        hashCode.HasValue
            ? hashCode.Value
            : (hashCode = hash(AsEnumerable())).Value;
    
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();
}
