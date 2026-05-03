using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// IteratorIOs are lazy, immutable sequences that can be consumed one item at a time.  They are functionally pure
/// unlike `IEnumerator` in the .NET framework.  
/// </summary>
/// <remarks>
/// <para>
/// All the language-ext collection types are written to support `IteratorIO` as a first-class citizen (by implementing 
/// the `IterableK` and/or the `IterableBackK` trait). That means you don't have to worry about the mutability problems
/// of `IEnumerator`.  You can just use them as you would any other collection type, and you can hold on to references 
/// mid-iteration, pass those references around to different threads, or anything you like, in the same way as any
/// regular immutable data-types.
/// </para>
/// <para>
/// The only time you need to be careful is if you construct an `IteratorIO` from a regular `IEnumerable`.  The reference
/// you get back is completely safe to pass around and use as normal.  But as soon as you try to consume the first
/// element, the `IEnumerable` will have to generate an `IEnumerator`, which is mutable and not guaranteed to be
/// thread-safe. 
/// </para>
/// <para>
/// In that situation you need to make sure you're not passing intermediate `IteratorIO` values around, and instead you
/// simply consume the iterable in one pass.  
/// </para>
/// <para>
/// This is the normal usage of enumerators, so it's not a big constraint, but it's worth understanding the limitation. 
/// </para>
/// <para>
/// NOTE: This type supports `IDisposable`, but it only needs disposing if you have constructed the `IteratorIO` from a
/// regular `IEnumerable`. And even then, only if the `IEnumerable` holds onto some resource during its yielding phase.
/// See <see cref="Using"/> for more details.
/// </para>
/// <para>
/// An example might be if you were iterating a set of results from a database or file system.  In that case, you would
/// want to dispose of the `IteratorIO` so that it can free up any underlying `IEnumerator`.
/// </para>
/// <para>
/// Calling `Dispose` on the `IteratorIO` when the `IteratorIO` hasn't been constructed from an `IEnumerable` will have no
/// effect. 
/// </para>
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
public abstract partial class IteratorIO<A> :
    IAsyncEnumerable<A>,
    IComparisonOperators<IteratorIO<A>, IteratorIO<A>, IO<bool>>,
    IDisposable,
    K<IteratorIO, A>
{
    /// <summary>
    /// Empty IteratorIO
    /// </summary>
    public static IteratorIO<A> Empty => Nil.Default;

    /// <summary>
    /// Consume the next item in the sequence
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will lazily consume the next item in the IteratorIO. `Head` will be `Exist〈A〉` if the IteratorIO
    /// is not empty, otherwise it will be `Nil〈A〉`.  `Tail` will be the remainder of the IteratorIO.
    /// </para> 
    /// </remarks>
    /// <example>
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing. The
    /// deconstructor simply calls `Next` to extract the head and tail of the IteratorIO:
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
    public abstract IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO();

    /// <summary>
    /// Consume the next item in the sequence
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will lazily consume the next item in the IteratorIO. `Head` will be `Exist〈A〉` if the IteratorIO
    /// is not empty, otherwise it will be `Nil〈A〉`.  `Tail` will be the remainder of the IteratorIO.
    /// </para> 
    /// </remarks>
    /// <example>
    /// It is possible to use the deconstructor in a for-loop to repeatedly consume the iterable thing. The
    /// deconstructor simply calls `Next` to extract the head and tail of the IteratorIO:
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
    public K<M, (A Head, IteratorIO<A> Tail)> NextM<M>()
        where M : MonadIO<M>, Alternative<M> =>
        NextIO() >> (ht => ht is (Exist<A> (var h), { } t)
                                ? M.Pure((h, t))
                                : M.Empty<(A, IteratorIO<A>)>());
    
    /// <summary>
    /// This will 'prime' an IteratorIO so that calling `Dispose` on the `IteratorIO` returned from this method will
    /// correctly release any backing resources. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// You only need to use this if your `IteratorIO` has been constructed from an `IEnumerable`.  And only if you're
    /// not consuming this IteratorIO using `foreach`.
    /// </para>
    /// <para>
    /// If you don't know whether your `IteratorIO` has been constructed from an `IEnumerable`, invoke this method on
    /// your `IteratorIO` just in case: for other `IteratorIO` types, this method will have no effect.  
    /// </para>
    /// <para>
    /// If your `IteratorIO` is a composition of other
    /// IteratorIOs (like if you zip two IteratorIOs, or you map, filter, etc.), then you can still call `Using` on the
    /// composed `IteratorIO` and it will flow through to the underlying IteratorIO(s). 
    /// </para>
    /// <para>
    /// For a deeper understanding: imagine that when an `IEnumerable` is lifted into an `IteratorIO`, it hasn't yet
    /// generated its `IEnumerator` (using `GetEnumerator()`), and so the `IteratorIO` that contains the `IEnumerable`
    /// has no resources to release yet.  
    /// </para>
    /// <para>
    /// When you start consuming the items from the `IteratorIO`, the first `(head, tail)` pair you get will have
    /// the tail `IteratorIO` carrying an `IEnumerator` that has been newly generated from the original lifted
    /// `IEnumerable`.
    /// </para>
    /// <para>
    /// That means the original `IteratorIO` that carried the `IEnumerable` is not the `IteratorIO` you want to call
    /// `Dispose` on.  It's the very first tail-`IteratorIO`.
    /// </para>
    /// <para>
    /// In that situation, it's quite difficult to stop, mid-iteration, to grab a reference to the first tail
    /// `IteratorIO`, and then somehow track that value until the end of the iteration, and then dispose of it!
    /// </para>
    /// <para>
    /// So, instead the `Using` method makes the 'first move' and generates the `IEnumerator`, which makes tracking
    /// which `IteratorIO` to dispose much simpler (and can be passed to a `using` expression).
    /// </para>
    /// <para>
    /// NOTE: If you're manually iterating over the `IteratorIO` using the deconstructor or `(head, tail) = Next()`, you
    /// can still call `Using` to get an initial disposable `IteratorIO`, but you don't have to, you can call `Dispose`
    /// manually on any of the subsequent tail `IteratorIO` instances you receive. This is most convenient when you're
    /// recursively iterating, and you only have the current `IteratorIO` instance. 
    /// </para>
    /// </remarks>
    /// <returns>Disposable `IteratorIO`</returns>
    public abstract IteratorIO<A> Using();

    /// <summary>
    /// Create an `IEnumerable` from an `IteratorIO`
    /// </summary>
    [Pure]
    public IO<IEnumerable<A>> AsEnumerable()
    {
        return IO.lift(go);
        IEnumerable<A> go(EnvIO e)
        {
            using var env  = e.Local;
            using var iter = Using();
            for (var i = iter; i.NextIO().Run(env) is (Exist<A> head, var tail); i = tail)
            {
                if (env.Token.IsCancellationRequested) yield break;
                yield return head.Value;
            }
        }
    }

    /// <summary>
    /// Create an `AsyncEnumerable` from an `IteratorIO`
    /// </summary>
    [Pure]
    public IO<IAsyncEnumerable<A>> AsAsyncEnumerable()
    {
        return IO.lift(go);
        async IAsyncEnumerable<A> go(EnvIO e)
        {
            using var env  = e.Local; 
            using var iter = Using();
            for (var i = iter; await i.NextIO().RunAsync(env) is (Exist<A> (var head), var tail); i = tail)
            {
                if (env.Token.IsCancellationRequested) yield break;
                yield return head;
            }
        }
    }

    /// <summary>
    /// Create an `Iterable` from an `Iterator`
    /// </summary>
    [Pure]
    public IterableIO<A> AsIterable() =>
        new (this);

    /// <summary>
    /// Forces evaluation of every item in the IteratorIO and then writes them to an `Arr` structure
    /// </summary>
    [Pure]
    public virtual IO<Arr<A>> ToArr()
    {
        return IO.liftVAsync(go);
        async ValueTask<Arr<A>> go()
        {
            var writer = ArrayWriter<A>.Init();
            await foreach (var head in this)
            {
                writer.Add(head);
            }

            return writer.ToArr();
        }
    }

    /// <summary>
    /// Wrap this IteratorIO in an IteratorIO that will cache the values as they're processed so
    /// that subsequent iterations use the cached values rather than the underlying IteratorIO.
    /// </summary>
    /// <remarks>The cache needs to retain the items in memory, so this should be used where there's a performance
    /// benefit to doing so: a trade-off between memory usage and the cost of re-running the IteratorIO.</remarks>
    /// <remarks>
    /// This is similar to `Strict` in that it caches the results, but `Strict` forces the entire sequence to
    /// evaluate immediately, whereas `OnceOnly` caches as it goes.
    /// </remarks>
    /// <returns>An IteratorIO that only iterates once</returns>
    [Pure]
    public IteratorIO<A> OnceOnly() =>
        new IteratorIO.OnceOnly<A>(this);

    /// <summary>
    /// Forces evaluation of every item in the IteratorIO and then caches them as a backing array which can be
    /// iterated.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is similar to `OnceOnly` in that it caches the results, but `OnceOnly` caches as it goes, rather than
    /// forcing the entire sequence to evaluate immediately.
    /// </para>
    /// <para>
    /// Any backing structure that has already been evaluated/is already strict, like if you lift an `Arr`, `HashMap`,
    /// `HashSet`, `Lst`, `Map`, or `Set` into an `IteratorIO`, will be returned as-is. 
    /// </para>
    /// </remarks>
    /// <returns></returns>
    [Pure]
    public virtual IteratorIO<A> Strict()
    {
        var arr = ToArr().Run();
        return IteratorIO.forward(arr);
    }
    
    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorIO<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorIO<B> Map<B>(Func<A, B> f) =>
        new IteratorIO<B>.OpMap<A>(this, f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorIO<B> Map<B>(Func<A, long, B> f, long start = 0) =>
        new IteratorIO<B>.OpMap2<A>(this, f, start);

    /// <summary>
    /// Map and filtering
    /// </summary>
    [Pure]
    public IteratorIO<B> Choose<B>(Func<A, Option<B>> f) =>
        new IteratorIO<B>.OpChoose<A>(this, f);

    /// <summary>
    /// Casts each value to the generic-type provided.  If the type-cast fails, the value is skipped.
    /// </summary>
    /// <typeparam name="B">Type to cast to</typeparam>
    /// <returns>IteratorIO with the values that were successfully cast.</returns>
    [Pure]
    public IteratorIO<B> Cast<B>() =>
        Choose(x => x is B b ? Some(b) : None);

    /// <summary>
    /// Filtering by predicate
    /// </summary>
    [Pure]
    public IteratorIO<A> Filter(Func<A, bool> f) =>
        new OpFilter(this, f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorIO<A> Where(Func<A, bool> f) =>
        new OpFilter(this, f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public IteratorIO<B> Bind<B>(Func<A, IteratorIO<B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public IteratorIO<B> Bind<B>(Func<A, K<IteratorIO, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public IteratorIO<C> SelectMany<B, C>(Func<A, IteratorIO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public IteratorIO<B> ApplyBack<B>(IteratorIO<Func<A, B>> ff) =>
        +ff.Bind(Map);

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public IteratorIO<A> Skip(long amount) =>
        new OpSkip(this, amount);

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public IteratorIO<A> SkipWhile(Func<A, bool> predicate) =>
        new OpSkipWhile(this, predicate);

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public IteratorIO<A> SkipUntil(Func<A, bool> predicate) =>
        new OpSkipUntil(this, predicate);

    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public IteratorIO<A> Take(long amount) =>
        new OpTake(this, amount);

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public IteratorIO<A> TakeWhile(Func<A, bool> predicate) =>
        new OpTakeWhile(this, predicate);

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public IteratorIO<A> TakeUntil(Func<A, bool> predicate) =>
        new OpTakeUntil(this, predicate);

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IteratorIO<A> Distinct() =>
        new IteratorIO.OpDistinct<EqDefault<A>, A>(this, []);

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IteratorIO<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        new IteratorIO.OpDistinct<EqDefault<A>, A>(this, []);

    /// <summary>
    /// Concatenate two IteratorIOs
    /// </summary>
    [Pure]
    public IteratorIO<A> Combine(IteratorIO<A> other) =>
        new OpCombine(this, other);

    /// <summary>
    /// Reverse the sequence of the IteratorIO
    /// </summary>
    /// <remarks>
    /// <para>
    /// The entire stream must be consumed before the elements, in reverse order, can be yielded.
    /// </para>
    /// <para>
    /// For infinite streams
    /// this will just fill up memory and will therefore kill your application, so be sure you understand the cost
    /// of reversing an IteratorIO stream.
    /// </para>
    /// <para>
    /// To avoid this, you can use an ordered data-structure that can support reversal without having to process
    /// every forward element first, like: `Arr`, `Lst`, `Map`, and `Set`.  
    /// </para> 
    /// </remarks>
    /// <returns>Reversed IteratorIO</returns>
    [Pure]
    public IteratorIO<A> Reverse() =>
        new OpReverse(this);

    /// <summary>
    /// Interleave two IteratorIO sequences together
    /// </summary>
    /// <remarks>
    /// Whilst there are items in both sequences, each is yielded, one after the other. Once one sequence runs
    /// out of items, the items that are remaining in the other sequence are yielded alone.
    /// </remarks>
    [Pure]
    public IteratorIO<A> Merge(IteratorIO<A> other) =>
        new OpMerge(this, other);

    /// <summary>
    /// Zips the items of two sequences together
    /// </summary>
    /// <remarks>
    /// The output sequence will be as long as the shortest input sequence.
    /// </remarks>
    [Pure]
    public IteratorIO<(A First, B Second)> Zip<B>(IteratorIO<B> other) =>
        new IteratorIO.OpZip<A, B>(this, other);

    /// <summary>
    /// Zips the items of two sequences together
    /// </summary>
    /// <remarks>
    /// The output sequence will be as long as the shortest input sequence.
    /// </remarks>
    [Pure]
    public IteratorIO<C> Zip<B, C>(IteratorIO<B> other, Func<A, B, C> join) =>
        new IteratorIO.OpZip<A, B, C>(this, other, join);

    /// <summary>
    /// Prepend an item to the beginning of the iterable sequence
    /// </summary>
    [Pure]
    public virtual IteratorIO<A> Prepend(A value) =>
        IteratorIO.cons(value, this);

    /// <summary>
    /// Append an item to the end of the iterable sequence
    /// </summary>
    [Pure]
    public virtual IteratorIO<A> Append(A value) =>
        new IteratorIO.Add<A>(this, [value]);

    /// <summary>
    /// Combine two sequences
    /// </summary>
    [Pure]
    public static IteratorIO<A> operator +(IteratorIO<A> ma, IteratorIO<A> mb) =>
        ma.Combine(mb);

    /// <summary>
    /// Prepend an item to the beginning of the iterable sequence
    /// </summary>
    [Pure]
    public static IteratorIO<A> operator +(A value, IteratorIO<A> mb) =>
        IteratorIO.cons(value, mb);

    /// <summary>
    /// Append an item to the end of the iterable sequence
    /// </summary>
    [Pure]
    public static IteratorIO<A> operator +(IteratorIO<A> ma, A value) =>
        ma.Append(value);

    /// <summary>
    /// Merge two sequences
    /// </summary>
    [Pure]
    public static IteratorIO<A> operator |(IteratorIO<A> ma, IteratorIO<A> mb) =>
        new OpAlt(ma, mb);

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
        // Only the IteratorIO.Enumerator and IteratorIO.AsyncEnumerator uses Dispose
    }
    
    /// <summary>
    /// Equality comparison
    /// </summary>
    [Pure]
    public static IO<bool> operator==(IteratorIO<A>? lhs, IteratorIO<A>? rhs) =>
        lhs?.Equals(rhs) ?? IO.pure(false);
    
    /// <summary>
    /// Non-equality comparison
    /// </summary>
    [Pure]
    public static IO<bool> operator!=(IteratorIO<A>? lhs, IteratorIO<A>? rhs) =>
        (lhs?.Equals(rhs) ?? IO.pure(false)).Map(not);

    [Pure]
    public static IO<bool> operator >(IteratorIO<A> left, IteratorIO<A> right) => 
        left.CompareTo(right).Map(c => c > 0);

    [Pure]
    public static IO<bool> operator >=(IteratorIO<A> left, IteratorIO<A> right) => 
        left.CompareTo(right).Map(c => c >= 0);

    [Pure]
    public static IO<bool> operator <(IteratorIO<A> left, IteratorIO<A> right) => 
        left.CompareTo(right).Map(c => c < 0);

    [Pure]
    public static IO<bool> operator <=(IteratorIO<A> left, IteratorIO<A> right) => 
        left.CompareTo(right).Map(c => c <= 0);
    
    /// <summary>
    /// Equality comparison
    /// </summary>
    [Pure]
    public IO<bool> Equals(IteratorIO<A>? rhs) =>
        Equals<EqDefault<A>>(rhs);

    /// <summary>
    /// Equality comparison
    /// </summary>
    [Pure]
    public IO<bool> Equals<EqA>(IteratorIO<A>? rhs)
        where EqA : Eq<A>
    {
        return rhs is null
                     ? IO.pure(false)
                     : +Monad.recur((this, rhs), go);

        K<IO, Next<(IteratorIO<A> lhs, IteratorIO<A> rhs), bool>> go((IteratorIO<A> lhs, IteratorIO<A> rhs) pair) =>
            (((Head<A> Head, IteratorIO<A> Tail) left, (Head<A> Head, IteratorIO<A> Tail) right) =>
                 (left.Head, right.Head) switch
                 {
                     (Exist<A> (var lh), Exist<A> (var rh)) =>
                         EqA.Equals(lh, rh)
                             ? Next.Loop<(IteratorIO<A> lhs, IteratorIO<A> rhs), bool>((left.Tail, right.Tail))
                             : Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), bool>(false),

                     (Exist<A>, _) or (_, Exist<A>) =>
                         Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), bool>(false),

                     _ => Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), bool>(true)
                 })
          * pair.lhs.NextIO()
          * pair.rhs.NextIO();
    }    

    [Pure]
    public IO<int> CompareTo(IteratorIO<A>? rhs) => 
        CompareTo<OrdDefault<A>>(rhs);

    [Pure]
    public IO<int> CompareTo<OrdA>(IteratorIO<A>? rhs)
        where OrdA : Ord<A>
    {
        return rhs is null
                   ? IO.pure(1)
                   : +Monad.recur((this, rhs), go);

        K<IO, Next<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>> go((IteratorIO<A> lhs, IteratorIO<A> rhs) pair) =>
            (((Head<A> Head, IteratorIO<A> Tail) left, (Head<A> Head, IteratorIO<A> Tail) right) =>
                 (left.Head, right.Head) switch
                 {
                     (Exist<A> (var lh), Exist<A> (var rh)) =>
                         OrdA.Compare(lh, rh) switch
                         {
                             0   => Next.Loop<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>((left.Tail, right.Tail)),
                             < 0 => Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>(-1),
                             > 0 => Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>(1),
                         },

                     (Exist<A>, _) =>
                         Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>(1),

                     (_, Exist<A>) =>
                         Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>(-1),

                     _ => Next.Done<(IteratorIO<A> lhs, IteratorIO<A> rhs), int>(0)
                 })
          * pair.lhs.NextIO()
          * pair.rhs.NextIO();
    }

    [Pure]
    public IteratorAsyncEnumeratorIO<A> GetAsyncEnumerator(CancellationToken cancellationToken = new()) =>
        new (this, EnvIO.New(token: cancellationToken));

    [Pure]
    public IteratorAsyncEnumeratorIO<A> GetAsyncEnumerator(EnvIO env) =>
        new (this, env.Local);

    [Pure]
    IAsyncEnumerator<A> IAsyncEnumerable<A>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        using var env = EnvIO.New(token: cancellationToken);
        return GetAsyncEnumerator(env);
    }

    [Pure]
    public override string ToString() =>
        "...";
}
