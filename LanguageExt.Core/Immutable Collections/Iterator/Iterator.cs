using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
/// <para>
/// The only time you need to be careful is if you construct an `Iterator` from a regular `IEnumerable`.  The reference
/// you get back is completely safe to pass around and use as normal.  But as soon as you try to consume the first
/// element, the `IEnumerable` will have to generate an `IEnumerator`, which is mutable and not guaranteed to be
/// thread-safe. 
/// </para>
/// <para>
/// In that situation you need to make sure you're not passing intermediate `Iterator` values around, and instead you
/// simply consume the iterable in one pass.  
/// </para>
/// <para>
/// This is the normal usage of enumerators, so it's not a big constraint, but it's worth understanding the limitation. 
/// </para>
/// <para>
/// NOTE: This type supports `IDisposable`, but it only needs disposing if you have constructed the `Iterator` from a
/// regular `IEnumerable`. And even then, only if the `IEnumerable` holds onto some resource during its yielding phase.
/// </para>
/// <para>
/// An example might be if you were iterating a set of results from a database or file system.  In that case, you would
/// want to dispose of the `Iterator` so that it can free up any underlying `IEnumerator`.
/// </para>
/// <para>
/// Calling `Dispose` on the `Iterator` when the `Iterator` hasn't been constructed from an `IEnumerable` will have no
/// effect. 
/// </para>
/// <para>
/// It may not be obvious which `Iterator` instance to `Dispose` as we create a new one for every tail during the
/// iteration process: in fact, any one of the instances can be disposed, and it will find the underlying `IEnumerator`
/// to `Dispose` and will do so only once.  
/// </para>
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
public abstract partial class Iterator<A> : 
    IEnumerable<A>,
    IEquatable<Iterator<A>>,
    IDisposable,
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
    /// This will lazily consume the next item in the iterator. `Head` will be `Exist〈A〉` if the iterator
    /// is not empty, otherwise it will be `Nil〈A〉`.  `Tail` will be the remainder of the iterator.
    /// </para> 
    /// </remarks>
    /// <example>
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing:
    /// <code>
    ///     for (var i = iter; i is (Exist&lt;A&gt; h, var t); i = t)
    ///     {
    ///         yield return h.Value;
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
    /// This will lazily consume the next item in the iterator. `Head` will be `Exist〈A〉` if the iterator
    /// is not empty, otherwise it will be `Nil〈A〉`.  `Tail` will be the remainder of the iterator.
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
    /// </example>
    public abstract (Head<A> Head, Iterator<A> Tail) Next();
    
    /// <summary>
    /// Consume the next item in the sequence but return only its tail, ignoring the head.
    /// </summary>
    [Pure]
    public Iterator<A> Tail
    {
        get
        {
            var (_, tail) = this;
            return tail;
        }
    }

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
    /// Create an `Iterable` from an `Iterator`
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() =>
        new IterableIterator<A>(this);
    
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
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<A> Filter(Func<A, bool> f) =>
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
        Map(x => +f(x)).Flatten();

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
        ff.Bind(Map);

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
    /// Combine two sequences
    /// </summary>
    public static Iterator<A> operator +(Iterator<A> ma, Iterator<A> mb) =>
        ma.Combine(mb);

    /// <summary>
    /// Combine two sequences
    /// </summary>
    public static Iterator<A> operator +(A ma, Iterator<A> mb) =>
        Iterator.ConsStrict(ma, mb);

    /// <summary>
    /// Combine two sequences
    /// </summary>
    public static Iterator<A> operator +(Iterator<A> ma, A mb) =>
        ma switch
        {
            Add add => add.More(mb),
            _       => new Add(ma, [mb])
        };

    /// <summary>
    /// Merge two sequences
    /// </summary>
    public static Iterator<A> operator |(Iterator<A> ma, Iterator<A> mb) =>
        new OpAlt(ma, mb);

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
        // Only the Iterator.Enumerator uses Dispose
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="obj">Other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public override bool Equals(object? obj) =>
        obj is Iterator<A> other && Equals(other);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">Other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public bool Equals(Iterator<A>? rhs)
    {
        if(rhs is null) return false;
        var lhs = this;
        while (true)
        {
            switch (lhs, rhs)
            {
                case ((Exist<A> (var lh), var lt), (Exist<A> (var rh), var rt)):
                    if (!EqDefault<A>.Equals(lh, rh))
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
    public IEnumerator<A> GetEnumerator() => 
        AsEnumerable().GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

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
        CollectionFormat.ToFullArrayString(this.AsEnumerable(), separator);

    const int OffsetBasis = -2128831035;
    const int Prime = 16777619;
}
