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

namespace LanguageExt;

/// <summary>
/// Lazy sequence
/// </summary>
/// <remarks>
/// This is a lightweight wrapper around `IEnumerable` that also implements traits
/// that make it play nice with other types in this library: Monad, Traversable, etc. 
/// </remarks>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
[CollectionBuilder(typeof(Iterable), nameof(Iterable.createRange))]
public abstract class Iterable<A> :
    IEnumerable<A>,
    Monoid<Iterable<A>>,
    IComparable<Iterable<A>>,
    IEqualityOperators<Iterable<A>, Iterable<A>, bool>,
    IAdditiveIdentity<Iterable<A>, Iterable<A>>,
    IComparisonOperators<Iterable<A>, Iterable<A>, bool>,
    IAdditionOperators<Iterable<A>, Iterable<A>, Iterable<A>>,
    K<Iterable, A>
{
    int? hashCode;

    public static Iterable<A> FromSpan(ReadOnlySpan<A> ma) =>
        new IterableEnumerable<A>(ma.ToArray().AsEnumerable());

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public abstract int Count();

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public abstract IEnumerable<A> AsEnumerable();
    

    /// <summary>
    /// Reverse the sequence
    /// </summary>
    [Pure]
    public abstract Iterable<A> Reverse();

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public virtual Iterable<A> Add(A item) =>
        new IterableAdd<A>(
            new SeqStrict<A>(new A[8], 8, 0, 0, 0),
            this,
            new SeqStrict<A>([item, default!, default!, default!, default!, default!, default!, default!], 0, 1, 0, 0)); 

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public virtual Iterable<A> Cons(A item) =>
        new IterableAdd<A>(
            new SeqStrict<A>([default!, default!, default!, default!, default!, default!, default!, item], 7, 1, 0, 0),
            this,
            new SeqStrict<A>(new A[8], 0, 0, 0, 0)); 

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public abstract Unit Iter(Action<A> f);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public abstract Unit Iter(Action<A, int> f);
    
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public abstract Iterable<B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public abstract Iterable<B> Map<B>(Func<A, int, B> f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public abstract Iterable<B> Bind<B>(Func<A, Iterable<B>> f);

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public abstract Iterable<A> Filter(Func<A, bool> f);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public virtual bool Equals(Iterable<A>? other) =>
        other is not null && Equals<EqDefault<A>>(other);

    [Pure]
    public static bool operator ==(Iterable<A>? lhs, Iterable<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => true,
            (null, _)    => false,
            (_, null)    => false,
            _            => lhs.Equals(rhs)
        };

    [Pure]
    public static bool operator !=(Iterable<A>? lhs, Iterable<A>? rhs) =>
        !(lhs == rhs);

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
    public Iterable<A> Combine(Iterable<A> y) =>
        Concat(y);

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(IEnumerable<A> items) =>
        Concat(new IterableEnumerable<A>(items));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(Iterable<A> items) =>
        (this, items) switch
        {
            (IterableConcat<A> l, IterableConcat<A> r) => new IterableConcat<A>(l.Items + r.Items),
            (IterableConcat<A> l, var r)               => new IterableConcat<A>(l.Items.Add(r)),
            (var l, IterableConcat<A> r)               => new IterableConcat<A>(l.Cons(r.Items)),
            var (l, r)                                 => new IterableConcat<A>(Seq(l, r))
        };
    
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
    public Iterable<S> Scan<S>(S state, Func<S, A, S> folder) 
    {
        return new IterableEnumerable<S>(Yield());
        IEnumerable<S> Yield()
        {
            yield return state;
            foreach (var item in AsEnumerable())
            {
                state = folder(state, item);
                yield return state;
            }
        }
    }

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
    public Iterable<S> ScanBack<S>(S state, Func<S, A, S> folder) =>
        Reverse().Scan(state, folder);
    
    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public Iterable<A> Distinct<EqA>()
        where EqA : Eq<A>
    {
        return new IterableEnumerable<A>(Yield());
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
    public Iterable<A> Distinct() =>
        Distinct<EqDefault<A>>();
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Iterable<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Iterable<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, K<Iterable, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, IEnumerable<B>> f) =>
        Bind(x => new IterableEnumerable<B>(f(x)));

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        this.Exists(_ => true);

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public Iterable<A> Intersperse(A value)
    {
        return new IterableEnumerable<A>(Yield());
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
        obj is Iterable<A> rhs
            ? CompareTo(rhs)
            : 1;

    [Pure]
    public int CompareTo(Iterable<A>? other) =>
        CompareTo<OrdDefault<A>>(other);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public virtual int CompareTo<OrdA>(Iterable<A>? rhs) 
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
    public virtual string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(AsEnumerable(), separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public virtual string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(AsEnumerable(), separator);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public virtual bool Equals<EqA>(Iterable<A> rhs) where EqA : Eq<A>
    {
        if (ReferenceEquals(this, rhs)) return true;
        using var iterA = GetEnumerator();
        using var iterB = rhs.AsEnumerable().GetEnumerator();
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

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    public Iterable<A> Skip(int amount) =>
        amount < 1
            ? this
            : new IterableEnumerable<A>(AsEnumerable().Skip(amount));

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    public Iterable<A> Take(int amount) =>
        amount < 1
            ? Empty
            : new IterableEnumerable<A>(AsEnumerable().Take(amount));

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, bool> pred)
    {
        return new IterableEnumerable<A>(Yield(AsEnumerable(), pred));
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
    public Iterable<A> TakeWhile(Func<A, int, bool> pred)
    {
        return new IterableEnumerable<A>(Yield(AsEnumerable(), pred));
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
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public Iterable<B> Cast<B>()
    {
        var seq = AsEnumerable();
        return seq is IEnumerable<B> mb
                   ? new IterableEnumerable<B>(mb)
                   : new IterableEnumerable<B>(Yield(seq));
        
        IEnumerable<B> Yield(IEnumerable<A> ma)
        {
            foreach (object? item in ma.AsEnumerable())
            {
                if (item is B b) yield return b;
            }
        }
    }

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<(A First, B Second)> Zip<B>(Iterable<B> rhs) =>
        new IterableEnumerable<(A First, B Second)>(AsEnumerable().Zip(rhs.AsEnumerable()));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<C> Zip<B, C>(Iterable<B> rhs, Func<A, B, C> zipper) =>
        new IterableEnumerable<C>(AsEnumerable().Zip(rhs.AsEnumerable(), zipper));


    /// <summary>
    /// Empty sequence
    /// </summary>
    [Pure]
    public static Iterable<A> Empty => 
        new IterableEnumerable<A>(Enumerable.Empty<A>());

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, Iterable<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(Iterable<A> x, Iterable<A> y) =>
        x.CompareTo(y) <= 0;
                
    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    public static implicit operator Iterable<A>(SeqEmpty _) =>
        Empty;

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Select<B>(Func<A, int, B> f) =>
        Map(f);

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Iterable<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, Iterable<B>> bind) =>
        Bind(bind);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, Iterable<B>> bind, Func<A, B, C> project) =>
        Bind(x  => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, IEnumerable<B>> bind) =>
        Bind(bind);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, IEnumerable<B>> bind, Func<A, B, C> project) =>
        Bind(x => new IterableEnumerable<B>(bind(x)).Map(y => project(x, y)));

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
        hashCode is null
            ? (hashCode = hash(AsEnumerable())).Value
            : hashCode.Value;

    /// <summary>
    /// Get the additive-identity, i.e. the monoid-zero.  Which is the empty sequence/
    /// </summary>
    public static Iterable<A> AdditiveIdentity => 
        Empty;
    
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();
}
