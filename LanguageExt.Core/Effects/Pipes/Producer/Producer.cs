using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Traits;
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
///     Void <==       <== Unit
///           |         |
///     Unit ==>       ==> OUT
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
    public static Producer<X, Unit> yieldAll<M, X>(IEnumerable<X> xs)
        where M : Monad<M> =>
        from x in many(xs)
        from _ in yield<X, M>(x)
        select unit;
        
    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IAsyncEnumerable<X> xs)
        where M : Monad<M> =>
        from x in many(xs)
        from _ in yield<X, M>(x)
        select unit;
        
    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IObservable<X> xs)
        where M : Monad<M> =>
        from x in many(xs)
        from _ in yield<X, M>(x)
        select unit;

    /// <summary>
    /// Repeat a monadic action indefinitely, yielding each result
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<A, M, Unit> repeatM<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        repeat(lift<A, M, A>(ma).Bind(yield<A, M>));
        
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
    public static Producer<OUT, M, Unit> merge<OUT, M>(Seq<Producer<OUT, M, Unit>> ms) 
        where M : Monad<M>
    {
        var prod = yieldAll<M, OUT>(go());
            
        var pipe = from fo in Pipe.awaiting<M, OUT, OUT>()
                   from nx in Pipe.yield<OUT, OUT, M>(fo) 
                   select nx;

        return prod | pipe;
            
        IEnumerable<OUT> go()
        {
            var       queue   = new ConcurrentQueue<OUT>();
            using var wait    = new AutoResetEvent(true);
            var       running = true;
            Error?    failed  = null;
                
            // Posts a value to the queue and triggers the merged producer's yield
            Unit post(OUT x)
            {
                queue.Enqueue(x);
                wait.Set();
                return default;
            }

            // Consumer that drains any Producer
            Consumer<OUT, M, Unit> enqueue() =>
                from _ in Consumer.awaiting<M, OUT>().Select(post)
                from r in enqueue()
                select default(Unit);
                
            // Compose the enqueue Consumer with the Producer to create an Effect that can be run 
            var mmt = ms.Map(m => m | enqueue()).Map(e => e.RunEffect()).ToArray();

            // TODO: DECIDE WHAT TO DO ABOUT THE FACT WE NOW KNOW NOTHING ABOUT THE INNER MONAD
            
            do
            {
                await wait.WaitOneAsync(lenv.CancellationToken).ConfigureAwait(false);
                while (queue.TryDequeue(out var item))
                {
                    yield return item;
                }
            }
            // Keep processing until we're cancelled or all of the Producers have stopped producing
            while (running && !lenv.CancellationToken.IsCancellationRequested);

            if (failed != null)
            {
                yield return failed;
            }
        }
    }
        
    /// <summary>
    /// Merge an array of queues into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component queues have completed</remarks>
    /// <param name="ms">Sequence of queues to merge</param>
    /// <returns>Queues merged into a single producer</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(params Queue<OUT, M, Unit>[] ms) 
        where M : Monad<M> =>
        merge(toSeq(ms.Map(m => (Producer<OUT, M, Unit>)m)));
 
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
