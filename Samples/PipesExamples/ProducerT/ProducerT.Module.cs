using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes2;

/// <summary>
/// `ProducerT` streaming producer monad-transformer
/// </summary>
public static class ProducerT
{
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yield<M, OUT>(OUT value) 
        where M : Monad<M> =>
        PipeT.yield<M, Unit, OUT>(value);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(IEnumerable<OUT> values)
        where M : Monad<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(IAsyncEnumerable<OUT> values)
        where M : Monad<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);
    
    /// <summary>
    /// Create a producer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> pure<OUT, M, A>(A value)
        where M : Monad<M> =>
        PipeT.pure<Unit, OUT, M, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> fail<OUT, E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        PipeT.fail<Unit, OUT, E, M, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> error<OUT, M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        PipeT.error<Unit, OUT, M, A>(value);
    
    /// <summary>
    /// Create a producer that yields nothing at all
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> empty<OUT, M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        PipeT.empty<Unit, OUT, M, A>();
    
    /// <summary>
    /// Create a producer that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> lift<OUT, M, A>(Func<A> f) 
        where M : Monad<M> =>
        PipeT.lift<Unit, OUT, M, A>(f);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftM<OUT, M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        PipeT.liftM<Unit, OUT, M, A>(ma);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftIO<OUT, M, A>(IO<A> ma) 
        where M : Monad<M> =>
        PipeT.liftIO<Unit, OUT, M, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(Func<ProducerT<OUT, M, A>> f) 
        where M : Monad<M> =>
        PipeT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(Func<ValueTask<ProducerT<OUT, M, A>>> f) 
        where M : Monad<M> =>
        PipeT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(ValueTask<ProducerT<OUT, M, A>> f) 
        where M : Monad<M> =>
        PipeT.liftT(f.Map(p => p.Proxy));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeat<OUT, M, A>(ProducerT<OUT, M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(ma.Proxy).ToProducer();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeat<OUT, M, A>(Schedule schedule, ProducerT<OUT, M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(schedule, ma.Proxy).ToProducer();

    /// <summary>
    /// Continually lift & repeat the provided operation
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeatM<OUT, M, A>(K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<Unit, OUT, M, A>(ma).ToProducer();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeatM<OUT, M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<Unit, OUT, M, A>(schedule, ma).ToProducer();
                
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> fold<OUT, M, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init,
        ProducerT<OUT, M, A> Item)
        where M : Monad<M> =>
        PipeT.fold(Time, Fold, Init, Item.Proxy);

    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> foldUntil<OUT, M, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        ProducerT<OUT, M, A> Item)
        where M : Monad<M> =>
        PipeT.foldUntil(Fold, Pred, Init, Item.Proxy);
        
    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> foldUntil<OUT, M, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        ProducerT<OUT, M, A> Item)
        where M : Monad<M> =>
        PipeT.foldUntil(Time, Fold, Pred, Init, Item.Proxy);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> foldWhile<OUT, M, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        ProducerT<OUT, M, A> Item)
        where M : Monad<M> =>
        PipeT.foldWhile(Fold, Pred, Init, Item.Proxy);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true` or the `Schedule` completes.
    /// Once `false`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> foldWhile<OUT, M, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        ProducerT<OUT, M, A> Item)
        where M : Monad<M> =>
        PipeT.foldWhile(Time, Fold, Pred, Init, Item.Proxy);
    
    /*
    /// <summary>
    /// Merge multiple producers
    /// </summary>
    /// <param name="producers"></param>
    /// <typeparam name="OUT"></typeparam>
    /// <typeparam name="M"></typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> merge<OUT, M>(Seq<ProducerT<OUT, M, Unit>> producers)
        where M : Monad<M>
    {
        if (producers.Count == 0) return pure<OUT, M, Unit>(default);
        var complete = new CountdownEvent(producers.Count);
        var wait     = new AutoResetEvent(false);
        var queue    = new ConcurrentQueue<OUT>();
        var effects  = producers.Traverse(runEffect);

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
            return default;
        }
        
        ConsumerT<OUT, M, Unit> receive() =>
            ConsumerT.awaiting<M, OUT>()
                     .Map(enqueue);

        Unit countDown()
        {
            complete.Signal();
            return default;
        }

        K<M, ForkIO<Unit>> runEffect(ProducerT<OUT, M, Unit> p) =>
            (from _1 in p | receive()
             let _2 = countDown()
             select Unit.Default)
           .Run()
           .ForkIO();    
    }    */
}
