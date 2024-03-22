using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
    public static Producer<X, M, Unit> yieldAll<M, X>(IEnumerable<X> xs)
        where M : Monad<M> =>
        new IteratorFoldable<Void, Unit, Unit, X, EnumerableM, X, M, Unit>(
            xs.AsEnumerableM(),
            yield<X, M>,
            () => Pure<X, M, Unit>(unit))
           .ToProducer();
    
    [Pure, MethodImpl(mops)]
    public static Producer<X, M, Unit> yieldAll<M, X>(IAsyncEnumerable<X> xs)
        where M : Monad<M> =>
        yieldAll<M, X>(xs.ToBlockingEnumerable());

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
    public static Producer<OUT, M, Unit> merge<OUT, M>(Seq<Producer<OUT, M, Unit>> ms)
        where M : Monad<M> =>
        ms switch
        {
            { IsEmpty     : true }               => lift<OUT, M, Unit>(M.Pure(unit)),
            { Tail.IsEmpty: true }               => ms.Head.Value!,
            { Head        : var h, Tail: var t } => 
                from x in h.Value!
                from xs in merge<OUT, M>(t)
                select x
        };
        
    /// <summary>
    /// Merge an array of queues into a single producer
    /// </summary>
    /// <remarks>The merged producer completes when all component queues have completed</remarks>
    /// <param name="ms">Sequence of queues to merge</param>
    /// <returns>Queues merged into a single producer</returns>
    public static Producer<OUT, M, Unit> merge<OUT, M>(params Queue<OUT, M, Unit>[] ms) 
        where M : Monad<M> =>
        merge(ms.AsEnumerableM().ToSeq().Map(m => (Producer<OUT, M, Unit>)m));
 
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
