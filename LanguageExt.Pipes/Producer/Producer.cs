using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Traits;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

/// <summary>
/// Producers can only `yield`
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Void ==       == Unit
///           |         |
///     Unit ==〉      ==〉OUT
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Producer
{
    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, R> Pure<OUT, M, R>(R value) 
        where M : Monad<M> =>
        new Pure<Void, Unit, Unit, OUT, M, R>(value).ToProducer();
        
    /// <summary>
    /// Send a value downstream (whilst in a producer)
    /// </summary>
    /// <remarks>
    /// This is the simpler version (fewer generic arguments required) of `yield` that works
    /// for producers. 
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, Unit> yield<OUT, M>(OUT value) 
        where M : Monad<M> =>
        respond<Void, Unit, Unit, OUT, M>(value).ToProducer();

    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IEnumerable<X> xs)
        where M : Monad<M> =>
        new IteratorFoldable<Void, Unit, Unit, X, Iterable, X, M, Unit>(
                xs.AsIterable(),
                yield<X, M>,
                () => Pure<X, M, Unit>(unit))
           .ToProducer();

    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IAsyncEnumerable<X> xs)
        where M : Monad<M> =>
        new IteratorAsyncEnumerable<Void, Unit, Unit, X, Iterable, X, M, Unit>(
                xs,
                yield<X, M>,
                () => Pure<X, M, Unit>(unit))
           .ToProducer();

    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IObservable<X> xs)
        where M : Monad<M> =>
        yieldAll<M, X>(xs.ToAsyncEnumerable(new CancellationToken()));

    /// <summary>
    /// Repeat a monadic action indefinitely, yielding each result
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<A, M, Unit> repeatM<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        repeat(lift<A, M, A>(ma).Bind(x => yield<A, M>(x)));
        
    /// <summary>
    /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, R> lift<OUT, M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        lift<Void, Unit, Unit, OUT, M, R>(ma).ToProducer();
    
    /// <summary>
    /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, R> liftIO<OUT, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        liftIO<Void, Unit, Unit, OUT, M, R>(ma).ToProducer();
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Producer<S, M, Unit> FoldUntil<S, M, A>(
        this Producer<S, M, A> ma, 
        S Initial, 
        Func<S, A, S> Fold, 
        Func<A, bool> UntilValue)
        where M : Monad<M>
    {
        var state = Initial;
        return ma.Bind(
            x =>
            {
                if (UntilValue(x))
                {
                    var nstate = state;
                    state = Initial;
                    return yield<S, M>(nstate);
                }
                else
                {
                    state = Fold(state, x);
                    return Pure<S, M, Unit>(unit);
                }
            });
    }
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Producer<S, M, Unit> FoldWhile<S, M, A>(
        this Producer<S, M, A> ma, 
        S Initial, 
        Func<S, A, S> Fold, 
        Func<A, bool> WhileValue)
        where M : Monad<M>
    {
        var state = Initial;
        return ma.Bind(
            x =>
            {
                if (WhileValue(x))
                {
                    state = Fold(state, x);
                    return Pure<S, M, Unit>(unit);
                }
                else
                {
                    var nstate = state;
                    state = Initial;
                    return yield<S, M>(nstate);
                }
            });
    }
         
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Producer<S, M, Unit> FoldUntil<S, M, A>(
        this Producer<S, M, A> ma, 
        S Initial, Func<S, A, S> Fold, 
        Func<S, bool> UntilState)
        where M : Monad<M>
    {
        var state = Initial;
        return ma.Bind(
            x =>
            {
                state = Fold(state, x);
                if (UntilState(state))
                {
                    var nstate = state;
                    state = Initial;
                    return yield<S, M>(nstate);
                }
                else
                {
                    return Pure<S, M, Unit>(unit);
                }
            });
    }
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Producer<S, M, Unit> FoldWhile<S, M, A>(
        this Producer<S, M, A> ma, 
        S Initial, 
        Func<S, A, S> Fold, 
        Func<S, bool> WhileState)
        where M : Monad<M>
    {
        var state = Initial;
        return ma.Bind(
            x =>
            {
                state = Fold(state, x);
                if (WhileState(state))
                {
                    return Pure<S, M, Unit>(unit);
                }
                else
                {
                    var nstate = state;
                    state = Initial;
                    return yield<S, M>(nstate);
                }
            });
    }

    /// <summary>
    /// Merge a sequence of producers into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component producers have completed</remarks>
    /// <param name="ms">Sequence of producers to merge</param>
    /// <returns>Merged producers</returns>
    public static Producer<OUT, M, Unit> merge_OLD<OUT, M>(Seq<Producer<OUT, M, Unit>> ms)
        where M : Monad<M> =>
        ms switch
        {
            { IsEmpty     : true }               => lift<OUT, M, Unit>(M.Pure(unit)),
            { Tail.IsEmpty: true }               => ms[0],
            { Head        : var h, Tail: var t } => 
                from x in h.ValueUnsafe()!
                from xs in merge<OUT, M>(t)
                select x
        };

    public static Producer<OUT, M, Unit> merge<OUT, M>(Seq<Producer<OUT, M, Unit>> ms)
        where M : Monad<M>
    {
        if (ms.Count == 0) return Pure<OUT, M, Unit>(unit);
        var complete = new CountdownEvent(ms.Count);
        var wait     = new AutoResetEvent(false);
        var queue    = new ConcurrentQueue<OUT>();
        var effects  = ms.Traverse(runEffect);

        return from t in liftIO<OUT, M, CancellationToken>(cancelToken)
               from _ in effects
               from r in yieldAll<M, OUT>(dequeue(t))
               select unit;

        async IAsyncEnumerable<OUT> dequeue([EnumeratorCancellation] CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await wait.WaitOneAsync(50, token).ConfigureAwait(false);
                    if (complete.IsSet) yield break;
                    if (token.IsCancellationRequested) yield break;
                    while (queue.TryDequeue(out var item))
                    {
                        yield return item;
                    }
                }
            }
            finally
            {
                wait.Dispose();
                complete.Dispose();
            }
        }
        
        Unit enqueue(OUT value)
        {
            queue?.Enqueue(value);
            wait?.Set();
            return unit;
        }
        
        Consumer<OUT, M, Unit> receive() =>
            from x in Consumer.awaiting<M, OUT>()
            let _ = enqueue(x)
            select unit;

        Unit countDown()
        {
            complete.Signal();
            return unit;
        }

        K<M, Unit> runEffect(Producer<OUT, M, Unit> p) =>
            (from _1 in p | receive()
             let _2 = countDown()
             select unit)
            .ToEffect()
            .RunEffect();    
    }
    
    /// <summary>
    /// Merge an array of queues into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component queues have completed</remarks>
    /// <param name="ms">Sequence of queues to merge</param>
    /// <returns>Queues merged into a single producer</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(params Queue<OUT, M, Unit>[] ms) 
        where M : Monad<M> =>
        merge(ms.AsIterable().ToSeq().Map(m => (Producer<OUT, M, Unit>)m));
 
    /// <summary>
    /// Merge an array of producers into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component producers have completed</remarks>
    /// <param name="ms">Sequence of producers to merge</param>
    /// <returns>Merged producers</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(params Producer<OUT, M, Unit>[] ms) 
        where M : Monad<M> =>
        merge(toSeq(ms));
 
    /// <summary>
    /// Merge an array of producers into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component producers have completed</remarks>
    /// <param name="ms">Sequence of producers to merge</param>
    /// <returns>Merged producers</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(params Proxy<Void, Unit, Unit, OUT, M, Unit>[] ms) 
        where M : Monad<M> =>
        merge(toSeq(ms).Map(m => m.ToProducer()));
 
    /// <summary>
    /// Merge a sequence of queues into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component queues have completed</remarks>
    /// <param name="ms">Sequence of queues to merge</param>
    /// <returns>Queues merged into a single producer</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(Seq<Queue<OUT, M, Unit>> ms) 
        where M : Monad<M> =>
        merge(ms.Map(m => (Producer<OUT, M, Unit>)m));
 
    /// <summary>
    /// Merge a sequence of producers into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component producers have completed</remarks>
    /// <param name="ms">Sequence of producers to merge</param>
    /// <returns>Merged producers</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(Seq<Proxy<Void, Unit, Unit, OUT, M, Unit>> ms) 
        where M : Monad<M> =>
        merge(ms.Map(m => m.ToProducer()));
}
