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
/// See <see cref="Using"/> for more details.
/// </para>
/// <para>
/// An example might be if you were iterating a set of results from a database or file system.  In that case, you would
/// want to dispose of the `Iterator` so that it can free up any underlying `IEnumerator`.
/// </para>
/// <para>
/// Calling `Dispose` on the `Iterator` when the `Iterator` hasn't been constructed from an `IEnumerable` will have no
/// effect. 
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
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing. The
    /// deconstructor simply calls `Next` to extract the head and tail of the iterator:
    /// <code>
    ///     for (var i = iter; i is (Exist&lt;A&gt; h, var t); i = t)
    ///     {
    ///         yield return h.Value;
    ///     }
    /// </code>
    /// Or, use `foreach`, which will also deal with the disposal properly:
    /// <code>
    ///     foreach (var value in iter.Using())
    ///     {
    ///         yield return value;
    ///     }
    /// </code>
    /// </example>
    /// <remarks>
    /// See <see cref="Using" /> documentation for best `IDisposable` practices.
    /// </remarks>
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
    /// Or, use `foreach`, which will also deal with the disposal properly:
    /// <code>
    ///     foreach (var value in iter.Using())
    ///     {
    ///         yield return value;
    ///     }
    /// </code>
    /// </example>
    /// <remarks>
    /// See <see cref="Using" /> documentation for best `IDisposable` practices.
    /// </remarks>
    public abstract (Head<A> Head, Iterator<A> Tail) Next();

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
    /// Or, use `foreach`, which will also deal with the disposal properly:
    /// <code>
    ///     foreach (var value in iter.Using())
    ///     {
    ///         yield return value;
    ///     }
    /// </code>
    /// </example>
    /// <remarks>
    /// See <see cref="Using" /> documentation for best `IDisposable` practices.
    /// </remarks>
    public abstract IO<(Head<A> Head, Iterator<A> Tail)> NextIO();

    /// <summary>
    /// This will 'prime' an iterator so that calling `Dispose` on the `Iterator` returned from this method will
    /// correctly release any backing resources. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// You only need to use this if your `Iterator` has been constructed from an `IEnumerable`.  And only if you're
    /// not consuming this iterator using `foreach`.
    /// </para>
    /// <para>
    /// If you don't know whether your `Iterator` has been constructed from an `IEnumerable`, invoke this method on
    /// your `Iterator` just in case: for other `Iterator` types, this method will have no effect.  
    /// </para>
    /// <para>
    /// If your `Iterator` is a composition of other
    /// iterators (like if you zip two iterators, or you map, filter, etc.), then you can still call `Using` on the
    /// composed `Iterator` and it will flow through to the underlying iterator(s). 
    /// </para>
    /// <para>
    /// For a deeper understanding: imagine that when an `IEnumerable` is lifted into an `Iterator`, it hasn't yet
    /// generated its `IEnumerator` (using `GetEnumerator()`), and so the `Iterator` that contains the `IEnumerable`
    /// has no resources to release yet.  
    /// </para>
    /// <para>
    /// When you start consuming the items from the `Iterator`, the first `(head, tail)` pair you get will have
    /// the tail `Iterator` carrying an `IEnumerator` that has been newly generated from the original lifted
    /// `IEnumerable`.
    /// </para>
    /// <para>
    /// That means the original `Iterator` that carried the `IEnumerable` is not the `Iterator` you want to call
    /// `Dispose` on.  It's the very first tail-`Iterator`.
    /// </para>
    /// <para>
    /// In that situation, it's quite difficult to stop, mid-iteration, to grab a reference to the first tail
    /// `Iterator`, and then somehow track that value until the end of the iteration, and then dispose of it!
    /// </para>
    /// <para>
    /// So, instead the `Using` method makes the 'first move' and generates the `IEnumerator`, which makes tracking
    /// which `Iterator` to dispose much simpler (and can be passed to a `using` expression).
    /// </para>
    /// <para>
    /// NOTE: If you're manually iterating over the `Iterator` using the deconstructor or `(head, tail) = Next()`, you
    /// can still call `Using` to get an initial disposable `Iterator`, but you don't have to, you can call `Dispose`
    /// manually on any of the subsequent tail `Iterator` instances you receive. This is most convenient when you're
    /// recursively iterating, and you only have the current `Iterator` instance. 
    /// </para>
    /// </remarks>
    /// <returns>Disposable `Iterator`</returns>
    public abstract Iterator<A> Using();

    /// <summary>
    /// Create an `IEnumerable` from an `Iterator`
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
         using var iter = Using();
         for (var i = iter; i is (Exist<A> head, var tail); i = tail)
         {
             yield return head.Value;
         }
    }

    /// <summary>
    /// Create an `AsyncEnumerable` from an `Iterator`
    /// </summary>
    [Pure]
    public IO<IAsyncEnumerable<A>> AsAsyncEnumerable()
    {
        return IO.lift(go);

        async IAsyncEnumerable<A> go(EnvIO e)
        {
            using var iter = Using();
            for (var i = iter; await i.NextIO().RunAsync(e) is (Exist<A> (var head), var tail); i = tail)
            {
                yield return head;
            }
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
        new IterableIterator<A>(this);

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
    /// Skip a specified number of items from the start of the iterator. 
    /// </summary>
    [Pure]
    public Iterator<A> Skip(int amount) =>
        new OpSkip(this, amount);

    /// <summary>
    /// Take a specified number of items from the start of the iterator. 
    /// </summary>
    [Pure]
    public Iterator<A> Take(int amount) =>
        new OpTake(this, amount);

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
        new Iterator.Add<A>(this, [value]);

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

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
        // Only the Iterator.Enumerator and Iterator.AsyncEnumerator uses Dispose
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

    const int OffsetBasis = -2128831035;
    const int Prime = 16777619;
}
