using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Iterators are lazy, immutable sequences that can be consumed one item at a time.  They are functionally pure
/// unlike `IEnumerator` in the .NET framework.  
/// </summary>
/// <remarks>
/// <para>
/// All the language-ext collection types are written to support `Iterator` as a first-class citizen (by implementing 
/// the `IterableK` and/or the `IterableBackK` trait). That means you don't have to worry about the mutability problems
/// of `IEnumerator`.  You can just use them as you would any other collection type, and you can hold on to references 
/// mid-iteration, pass those references around to different threads, or anything you like, in the same way as any
/// regular immutable data-types.
/// </para>
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
public abstract partial class Iterator<A> :
    IEnumerable<A>,
    IComparable<Iterator<A>>,
    IComparisonOperators<Iterator<A>, Iterator<A>, bool>,
    IEquatable<Iterator<A>>,
    K<Iterator, A>
{
    /// <summary>
    /// Empty iterator
    /// </summary>
    public static readonly Iterator<A> Empty = new Nil();

    /// <summary>
    /// Consume the next item in the sequence
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will lazily consume the next item in the iterator. `head` will be `Exist〈A〉` if the iterator
    /// is not-empty, otherwise it will be `Nil〈A〉`.  `tail` will be the remainder of the iterator.
    /// </para> 
    /// </remarks>
    /// <example>
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing. The
    /// deconstructor simply calls `Next` to extract the head and tail of the iterator:
    /// <code>
    ///     for (var i = iter; i is (Exist&lt;A&gt; h, var t); i = t)
    ///     {
    ///         yield return h.Value;
    ///     }
    /// </code>
    /// Or, use `foreach`:
    /// <code>
    ///     foreach (var value in iter)
    ///     {
    ///         yield return value;
    ///     }
    /// </code>
    /// </example>
    public void Deconstruct(out Head<A> head, out Iterator<A> tail)
    {
        var (h, t) = Next();
        head = h;
        tail = t;
    }

    /// <summary>
    /// Consume the next item in the sequence
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will lazily consume the next item in the iterator. `head` will be `Exist〈A〉` if the iterator
    /// is not-empty, otherwise it will be `Nil〈A〉`.  `tail` will be the remainder of the iterator.
    /// </para> 
    /// </remarks>
    /// <example>
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing. The
    /// deconstructor simply calls `Next` to extract the head and tail of the iterator:
    /// <code>
    ///     for (var i = iter; i.Next() is (Exist&lt;A&gt; h, var t); i = t)
    ///     {
    ///         yield return h.Value;
    ///     }
    /// </code>
    /// Or, use `foreach`:
    /// <code>
    ///     foreach (var value in iter)
    ///     {
    ///         yield return value;
    ///     }
    /// </code>
    /// </example>
    public abstract (Head<A> Head, Iterator<A> Tail) Next();

    /// <summary>
    /// Create an `IEnumerable` from an `Iterator`
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
         for (var i = this; i is (Exist<A> head, var tail); i = tail)
         {
             yield return head.Value;
         }
    }

    /// <summary>
    /// Forces evaluation of every item in the iterator and then writes them to an `Arr` structure
    /// </summary>
    [Pure]
    public virtual Arr<A> ToArr()
    {
        var writer = ArrayWriter<A>.Init();
        foreach (var head in this)
        {
            writer.Add(head);
        }
        return writer.ToArr();
    }

    /// <summary>
    /// Create an `Iterable` from an `Iterator`
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() =>
        new (this);

    /// <summary>
    /// Wrap this iterator in an iterator that will cache the values as they're processed so
    /// that subsequent iterations use the cached values rather than the underlying iterator.
    /// </summary>
    /// <remarks>The cache needs to retain the items in memory, so this should be used where there's a performance
    /// benefit to doing so: a trade-off between memory usage and the cost of re-running the iterator.</remarks>
    /// <remarks>
    /// This is similar to `Strict` in that it caches the results, but `Strict` forces the entire sequence to
    /// evaluate immediately, whereas `OnceOnly` caches as it goes.
    /// </remarks>
    /// <returns>An iterator that only iterates once</returns>
    [Pure]
    public Iterator<A> OnceOnly() =>
        new Iterator.OnceOnly<A>(this);

    /// <summary>
    /// Forces evaluation of every item in the iterator and then caches them as a backing array which can be
    /// iterated.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is similar to `OnceOnly` in that it caches the results, but `OnceOnly` caches as it goes, rather than
    /// forcing the entire sequence to evaluate immediately.
    /// </para>
    /// <para>
    /// Any backing structure that has already been evaluated/is already strict, like if you lift an `Arr`, `HashMap`,
    /// `HashSet`, `Lst`, `Map`, or `Set` into an `Iterator`, will be returned as-is. 
    /// </para>
    /// </remarks>
    /// <returns></returns>
    [Pure]
    public virtual Iterator<A> Strict()
    {
        var arr = ToArr();
        return new IterArr(arr, 0, arr.Count);
    }
    
    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<B> Map<B>(Func<A, B> f) =>
        new Iterator<B>.OpMap<A>(this, f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<B> Map<B>(Func<A, long, B> f, long offset = 0) =>
        new Iterator<B>.OpMap2<A>(this, f, offset);

    /// <summary>
    /// Map and filtering
    /// </summary>
    [Pure]
    public Iterator<B> Choose<B>(Func<A, Option<B>> f) =>
        new Iterator<B>.OpChoose<A>(this, f);

    /// <summary>
    /// Casts each value to the generic-type provided.  If the type-cast fails, the value is skipped.
    /// </summary>
    /// <typeparam name="B">Type to cast to</typeparam>
    /// <returns>Iterator with the values that were successfully cast.</returns>
    [Pure]
    public Iterator<B> Cast<B>() =>
        Choose(x => x is B b ? Some(b) : None);

    /// <summary>
    /// Filtering by predicate
    /// </summary>
    [Pure]
    public Iterator<A> Filter(Func<A, bool> f) =>
        new OpFilter(this, f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<A> Where(Func<A, bool> f) =>
        new OpFilter(this, f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public Iterator<B> Bind<B>(Func<A, Iterator<B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public Iterator<B> Bind<B>(Func<A, K<Iterator, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public Iterator<C> SelectMany<B, C>(Func<A, Iterator<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public Iterator<B> ApplyBack<B>(Iterator<Func<A, B>> ff) =>
        ff.Bind(f => Map(f));

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterator<A> Skip(long amount) =>
        new OpSkip(this, amount);

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterator<A> SkipWhile(Func<A, bool> predicate) =>
        new OpSkipWhile(this, predicate);

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterator<A> SkipUntil(Func<A, bool> predicate) =>
        new OpSkipUntil(this, predicate);

    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterator<A> Take(long amount) =>
        new OpTake(this, amount);

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterator<A> TakeWhile(Func<A, bool> predicate) =>
        new OpTakeWhile(this, predicate);

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterator<A> TakeUntil(Func<A, bool> predicate) =>
        new OpTakeUntil(this, predicate);

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public Iterator<A> Distinct() =>
        Distinct<EqDefault<A>>();

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public Iterator<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        new Iterator.OpDistinct<EqA, A>(this, []);

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    internal Iterator<A> Distinct<EqA>(ReadOnlySpan<A> seen)
        where EqA : Eq<A> =>
        new Iterator.OpDistinct<EqA, A>(this, toHashSet<EqA, A>(seen));

    /// <summary>
    /// Concatenate two iterators
    /// </summary>
    [Pure]
    public Iterator<A> Combine(Iterator<A> other) =>
        new OpCombine(this, other);

    /// <summary>
    /// Reverse the sequence of the iterator
    /// </summary>
    /// <remarks>
    /// <para>
    /// The entire stream must be consumed before the elements, in reverse order, can be yielded.
    /// </para>
    /// <para>
    /// For infinite streams
    /// this will just fill up memory and will therefore kill your application, so be sure you understand the cost
    /// of reversing an iterator stream.
    /// </para>
    /// <para>
    /// To avoid this, you can use an ordered data-structure that can support reversal without having to process
    /// every forward element first, like: `Arr`, `Lst`, `Map`, and `Set`.  
    /// </para> 
    /// </remarks>
    /// <returns>Reversed iterator</returns>
    [Pure]
    public Iterator<A> Reverse() =>
        new OpReverse(this);

    /// <summary>
    /// Interleave two iterator sequences together
    /// </summary>
    /// <remarks>
    /// Whilst there are items in both sequences, each is yielded, one after the other. Once one sequence runs
    /// out of items, the items that are remaining in the other sequence are yielded alone.
    /// </remarks>
    [Pure]
    public Iterator<A> Merge(Iterator<A> other) =>
        new OpMerge(this, other);

    /// <summary>
    /// Zips the items of two sequences together
    /// </summary>
    /// <remarks>
    /// The output sequence will be as long as the shortest input sequence.
    /// </remarks>
    [Pure]
    public Iterator<(A First, B Second)> Zip<B>(Iterator<B> other) =>
        new Iterator.OpZip<A, B>(this, other);

    /// <summary>
    /// Zips the items of two sequences together
    /// </summary>
    /// <remarks>
    /// The output sequence will be as long as the shortest input sequence.
    /// </remarks>
    [Pure]
    public Iterator<C> Zip<B, C>(Iterator<B> other, Func<A, B, C> join) =>
        new Iterator.OpZip<A, B, C>(this, other, join);

    /// <summary>
    /// Prepend an item to the beginning of the iterable sequence
    /// </summary>
    [Pure]
    public virtual Iterator<A> Prepend(A value) =>
        Iterator.cons(value, this);

    /// <summary>
    /// Append an item to the end of the iterable sequence
    /// </summary>
    [Pure]
    public virtual Iterator<A> Append(A value) =>
        new Iterator.Add<A>([], this, [value]);

    /// <summary>
    /// Combine two sequences
    /// </summary>
    public static Iterator<A> operator +(Iterator<A> ma, Iterator<A> mb) =>
        ma.Combine(mb);

    /// <summary>
    /// Prepend an item to the beginning of the iterable sequence
    /// </summary>
    public static Iterator<A> operator +(A value, Iterator<A> mb) =>
        Iterator.cons(value, mb);

    /// <summary>
    /// Append an item to the end of the iterable sequence
    /// </summary>
    public static Iterator<A> operator +(Iterator<A> ma, A value) =>
        ma.Append(value);

    /// <summary>
    /// Merge two sequences
    /// </summary>
    public static Iterator<A> operator |(Iterator<A> ma, Iterator<A> mb) =>
        new OpAlt(ma, mb);

    public int CompareTo(Iterator<A>? other) =>
        CompareTo<OrdDefault<A>>(other);

    public int CompareTo<OrdA>(Iterator<A>? rhs) 
        where OrdA : Ord<A>
    {
        if(rhs is null) return 1;
        var lhs = this;
        while (true)
        {
            switch (lhs, rhs)
            {
                case ((Exist<A> (var lh), var lt), (Exist<A> (var rh), var rt)):
                    switch(OrdA.Compare(lh, rh))
                    {
                        case 0:
                            lhs = lt;
                            rhs = rt;
                            break;
                        
                        case > 0:
                            return 1;
                        
                        default:
                            return -1;
                    }
                    break;

                case ((Exist<A>, _), _):
                    return 1;
                    
                case (_, (Exist<A>, _)):
                    return -1;

                default:
                    return 0; // end of sequence
            }
        }
    }
    
    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="obj">The other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public override bool Equals(object? obj) =>
        obj is Iterator<A> other && Equals(other);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">The other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public bool Equals(Iterator<A>? rhs) =>
        Equals<EqDefault<A>>(rhs);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">The other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public bool Equals<EqA>(Iterator<A>? rhs)
        where EqA : Eq<A>
    {
        if(rhs is null) return false;
        var lhs = this;
        while (true)
        {
            switch (lhs, rhs)
            {
                case ((Exist<A> (var lh), var lt), (Exist<A> (var rh), var rt)):
                    if (!EqA.Equals(lh, rh))
                    {
                        return false;
                    }
                    lhs = lt;
                    rhs = rt;
                    break;

                case ((Exist<A>, _), _):
                case (_, (Exist<A>, _)):
                    return false;

                default:
                    return true;
            }
        }
    }

    [Pure]
    public override int GetHashCode()
    {
        var iter = this;
        var hash = OffsetBasis;
        while (iter is (Exist<A> (var head), var tail))
        {
            var itemHash = head?.GetHashCode() ?? 0;
            unchecked
            {
                hash = (hash ^ itemHash) * Prime;
            }
            iter = tail;
        }
        return hash;
    }

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns></returns>
    [Pure]
    public IteratorEnumerator<A> GetEnumerator() => 
        new (this);

    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        GetEnumerator().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator().GetEnumerator();

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

    public static bool operator ==(Iterator<A>? left, Iterator<A>? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Iterator<A>? left, Iterator<A>? right) =>
        !(left == right);

    public static bool operator >(Iterator<A> left, Iterator<A> right) => 
        left.CompareTo(right) > 0;

    public static bool operator >=(Iterator<A> left, Iterator<A> right) => 
        left.CompareTo(right) >= 0;

    public static bool operator <(Iterator<A> left, Iterator<A> right) => 
        left.CompareTo(right) < 0;

    public static bool operator <=(Iterator<A> left, Iterator<A> right) => 
        left.CompareTo(right) <= 0;
    
    const int OffsetBasis = -2128831035;
    const int Prime = 16777619;
}
