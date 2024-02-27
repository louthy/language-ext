using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Lazy sequence
/// </summary>
/// <remarks>
/// This is a lightweight wrapper around `IEnumerable` that also implements traits
/// that make it play nice with other types in this library: Monad, Traversable, etc. 
/// </remarks>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
[CollectionBuilder(typeof(EnumerableM), nameof(EnumerableM.createRange))]
public sealed record EnumerableM<A>(IEnumerable<A> runEnumerable) :
    IEnumerable<A>,
    Monoid<EnumerableM<A>>,
    K<EnumerableM, A>
{
    /// <summary>
    /// Empty sequence
    /// </summary>
    public static EnumerableM<A> Empty { get; } = new(Enumerable.Empty<A>());

    /// <summary>
    /// Constructor from lazy sequence
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM(ReadOnlySpan<A> ma) : 
        this(ma.ToArray().AsEnumerable()) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Count() =>
        runEnumerable.Count();
    
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Concat(IEnumerable<A> items) =>
        new(runEnumerable.ConcatFast(items));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Concat(EnumerableM<A> items) =>
        new(runEnumerable.ConcatFast(items));

    /// <summary>
    /// Prepend an item to the sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EnumerableM<A> Cons(A value)
    {
        return new(go());
        IEnumerable<A> go()
        {
            yield return value;
            foreach (var item in runEnumerable)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Prepend an item to the sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal EnumerableM<A> Add(A value)
    {
        return new(go());
        IEnumerable<A> go()
        {
            foreach (var item in runEnumerable)
            {
                yield return item;
            }
            yield return value;
        }
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<A> AsEnumerable() => 
        runEnumerable;

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit Iter(Action<A> f) =>
        runEnumerable.Iter(f);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// </remarks>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, EnumerableM<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public EnumerableM<B> Map<B>(Func<A, B> f) =>
        new(runEnumerable.Select(f));
        
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<B> Select<B>(Func<A, B> f) =>
        new(runEnumerable.Select(f));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public EnumerableM<B> Bind<B>(Func<A, EnumerableM<B>> f)
    {
        return new EnumerableM<B>(go(this, f));
        
        static IEnumerable<B> go(EnumerableM<A> ma, Func<A, EnumerableM<B>> bnd)
        {
            foreach (var a in ma)
            {
                foreach (var b in bnd(a))
                {
                    yield return b;
                }
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
    public EnumerableM<B> Bind<B>(Func<A, K<EnumerableM, B>> f)
    {
        static IEnumerable<B> go(K<EnumerableM, A> ma, Func<A, K<EnumerableM, B>> bnd)
        {
            foreach (var a in ma.As())
            {
                foreach (var b in bnd(a).As())
                {
                    yield return b;
                }
            }
        }
        return new EnumerableM<B>(go(this, f));
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public EnumerableM<C> SelectMany<B, C>(Func<A, EnumerableM<B>> bind, Func<A, B, C> project)
    {
        static IEnumerable<C> Yield(EnumerableM<A> ma, Func<A, EnumerableM<B>> bnd, Func<A, B, C> prj)
        {
            foreach (var a in ma)
            {
                foreach (var b in bnd(a))
                {
                    yield return prj(a, b);
                }
            }
        }
        return new EnumerableM<C>(Yield(this, bind, project));
    }

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public EnumerableM<A> Filter(Func<A, bool> f)
    {
        return new EnumerableM<A>(new SeqLazy<A>(Yield(this, f)));
        static IEnumerable<A> Yield(EnumerableM<A> items, Func<A, bool> f)
        {
            foreach (var item in items)
            {
                if (f(item))
                {
                    yield return item;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Fold the sequence from the first item to the last
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold function</param>
    /// <returns>Aggregated state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S Fold<S>(S state, Func<S, A, S> f) =>
        runEnumerable.Fold(state, f);

    /// <summary>
    /// Fold the sequence from the last item to the first.  For 
    /// sequences that are not lazy and are less than 5000 items
    /// long, FoldBackRec is called instead, because it is faster.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="f">Fold function</param>
    /// <returns>Aggregated state</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public S FoldBack<S>(S state, Func<S, A, S> f) =>
        runEnumerable.FoldBack(state, f);

    /// <summary>
    /// Returns true if the supplied predicate returns true for any
    /// item in the sequence.  False otherwise.
    /// </summary>
    /// <param name="f">Predicate to apply</param>
    /// <returns>True if the supplied predicate returns true for any
    /// item in the sequence.  False otherwise.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists(Func<A, bool> f) =>
        runEnumerable.Exists(f);

    /// <summary>
    /// Returns true if the supplied predicate returns true for all
    /// items in the sequence.  False otherwise.  If there is an 
    /// empty sequence then true is returned.
    /// </summary>
    /// <param name="f">Predicate to apply</param>
    /// <returns>True if the supplied predicate returns true for all
    /// items in the sequence.  False otherwise.  If there is an 
    /// empty sequence then true is returned.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ForAll(Func<A, bool> f) =>
        runEnumerable.ForAll(f);

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Any() =>
        runEnumerable.Any();

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Intersperse(A value) =>
        new(runEnumerable.Intersperse(value));

    /// <summary>
    /// Get the hash code for all of the items in the sequence, or 0 if empty
    /// </summary>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return runEnumerable.GetHashCode();
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj) =>
        obj switch
        {
            EnumerableM<A> s         => CompareTo(s),
            IEnumerable<A> e => CompareTo(toSeq(e)),
            _                => 1
        };

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(runEnumerable);

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(this, separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(this, separator);

    [Pure]
    public EnumerableM<A> Append(EnumerableM<A> y) =>
        this + y;

    /// <summary>
    /// Append operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EnumerableM<A> operator +(EnumerableM<A> x, EnumerableM<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Ordering operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(EnumerableM<A> x, EnumerableM<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(EnumerableM<A> x, EnumerableM<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(EnumerableM<A> x, EnumerableM<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(EnumerableM<A> x, EnumerableM<A> y) =>
        x.CompareTo(y) <= 0;

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(EnumerableM<A> rhs) where EqA : Eq<A>
    {
        if (ReferenceEquals(this, rhs)) return true;
        // Iterate through both sides
        using var iterA = GetEnumerator();
        using var iterB = rhs.GetEnumerator();
        while (iterA.MoveNext())
        {
            if (iterB.MoveNext())
            {
                if(!EqA.Equals(iterA.Current, iterB.Current)) return false;
            }
            else
            {
                return false;
            }
        }
        if (iterB.MoveNext())
        {
            return false;
        }
        return true;
    }

    public bool Equals(EnumerableM<A>? other) =>
        other is { } rhs && Equals<EqDefault<A>>(rhs);

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Skip(int amount) =>
        amount < 1
            ? this
            : new EnumerableM<A>(runEnumerable.Skip(amount));

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> Take(int amount) =>
        amount < 1
            ? Empty
            : new EnumerableM<A>(runEnumerable.Take(amount));

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public EnumerableM<A> TakeWhile(Func<A, bool> pred)
    {
        return new EnumerableM<A>(go(runEnumerable, pred));
        IEnumerable<A> go(IEnumerable<A> xs, Func<A, bool> f)
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
    public EnumerableM<A> TakeWhile(Func<A, int, bool> pred)
    {
        return new EnumerableM<A>(go(runEnumerable, pred));
        IEnumerable<A> go(IEnumerable<A> xs, Func<A, int, bool> f)
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

    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    public (EnumerableM<A> First, EnumerableM<A> Second) Partition(Func<A, bool> predicate)
    {
        var f = List<A>();
        var s = List<A>();
        foreach (var item in this)
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
        return (new EnumerableM<A>(f), new EnumerableM<A>(s));
    }

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(EnumerableM<A> rhs) where OrdA : Ord<A>
    {
        // Iterate through both sides
        using var iterA = GetEnumerator();
        using var iterB = rhs.GetEnumerator();
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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<A> GetEnumerator() =>
        runEnumerable.GetEnumerator();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() =>
        runEnumerable.GetEnumerator();

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator EnumerableM<A>(SeqEmpty _) =>
        Empty;

    [Pure]
    public EnumerableM<B> Cast<B>()
    {
        IEnumerable<B> Yield(EnumerableM<A> ma)
        {
            foreach (object? item in ma)
            {
                if( item is B b) yield return b;
            }
        }

        return runEnumerable is IEnumerable<B> mb
                   ? new EnumerableM<B>(mb)
                   : new EnumerableM<B>(Yield(this));
    }
}
