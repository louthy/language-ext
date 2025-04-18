using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

public static class ProducerT
{
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yield<M, OUT>(OUT value) 
        where M : MonadIO<M> =>
        PipeT.yield<M, Unit, OUT>(value);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(IEnumerable<OUT> values)
        where M : MonadIO<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(IAsyncEnumerable<OUT> values)
        where M : MonadIO<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(Source<OUT> values)
        where M : MonadIO<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(SourceT<M, OUT> values)
        where M : MonadIO<M>, Alternative<M> =>
        PipeT.yieldAll<M, Unit, OUT>(values);
    
    /// <summary>
    /// Evaluate the `M` monad repeatedly, yielding its bound values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldRepeat<M, OUT>(K<M, OUT> ma)
        where M : MonadIO<M> =>
        PipeT.yieldRepeat<M, Unit, OUT>(ma);

    /// <summary>
    /// Evaluate the `IO` monad repeatedly, yielding its bound values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldRepeatIO<M, OUT>(IO<OUT> ma)
        where M : MonadIO<M> =>
        PipeT.yieldRepeatIO<M, Unit, OUT>(ma);
    
    /// <summary>
    /// Create a producer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> pure<OUT, M, A>(A value)
        where M : MonadIO<M> =>
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
        where M : MonadIO<M>, Fallible<E, M> =>
        PipeT.fail<Unit, OUT, E, M, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> error<OUT, M, A>(Error value) 
        where M : MonadIO<M>, Fallible<M> =>
        PipeT.error<Unit, OUT, M, A>(value);
    
    /// <summary>
    /// Create a producer that yields nothing at all
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> empty<OUT, M, A>() 
        where M : MonadIO<M>, MonoidK<M> =>
        PipeT.empty<Unit, OUT, M, A>();
    
    /// <summary>
    /// Create a producer that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> lift<OUT, M, A>(Func<A> f) 
        where M : MonadIO<M> =>
        PipeT.lift<Unit, OUT, M, A>(f);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftM<OUT, M, A>(K<M, A> ma) 
        where M : MonadIO<M> =>
        PipeT.liftM<Unit, OUT, M, A>(ma);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftIO<OUT, M, A>(IO<A> ma) 
        where M : MonadIO<M> =>
        PipeT.liftIO<Unit, OUT, M, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(Func<ProducerT<OUT, M, A>> f) 
        where M : MonadIO<M> =>
        PipeT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(Func<ValueTask<ProducerT<OUT, M, A>>> f) 
        where M : MonadIO<M> =>
        PipeT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftT<OUT, M, A>(ValueTask<ProducerT<OUT, M, A>> f) 
        where M : MonadIO<M> =>
        PipeT.liftT(f.Map(p => p.Proxy));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeat<OUT, M, A>(ProducerT<OUT, M, A> ma)
        where M : MonadIO<M> =>
        PipeT.repeat(ma.Proxy).ToProducer();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeat<OUT, M, A>(Schedule schedule, ProducerT<OUT, M, A> ma)
        where M : MonadIO<M> =>
        PipeT.repeat(schedule, ma.Proxy).ToProducer();

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeatM<OUT, M, A>(K<M, A> ma)
        where M : MonadIO<M> =>
        PipeT.repeatM<Unit, OUT, M, A>(ma).ToProducer();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> repeatM<OUT, M, A>(Schedule schedule, K<M, A> ma)
        where M : MonadIO<M> =>
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
        where M : MonadIO<M> =>
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
        where M : MonadIO<M> =>
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
        where M : MonadIO<M> =>
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
        where M : MonadIO<M> =>
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
        where M : MonadIO<M> =>
        PipeT.foldWhile(Time, Fold, Pred, Init, Item.Proxy);

    /// <summary>
    /// Merge multiple producers
    /// </summary>
    /// <param name="producers">Producers to merge</param>
    /// <param name="settings">Buffer settings</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns>Merged producer</returns>
    public static ProducerT<OUT, M, Unit> merge<OUT, M>(
        params ProducerT<OUT, M, Unit>[] producers)
        where M : MonadIO<M> =>
        merge(toSeq(producers));
    
    /// <summary>
    /// Merge multiple producers
    /// </summary>
    /// <param name="producers">Producers to merge</param>
    /// <param name="settings">Buffer settings</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns>Merged producer</returns>
    public static ProducerT<OUT, M, Unit> merge<OUT, M>(
        Seq<ProducerT<OUT, M, Unit>> producers, 
        Buffer<OUT>? settings = null)
        where M : MonadIO<M>
    {
        if (producers.Count == 0) return pure<OUT, M, Unit>(default);

        return from Conduit in Pure(Conduit.spawn(settings ?? Buffer<OUT>.Unbounded))
               from forks   in forkEffects(producers, Conduit)
               from _       in Conduit.ToProducerT<M>()
               from x       in forks.Traverse(f => f.Cancel).As()
               select unit;
    }

    static K<M, Seq<ForkIO<Unit>>> forkEffects<M, OUT>(
        Seq<ProducerT<OUT, M, Unit>> producers,
        Conduit<OUT, OUT> Conduit)
        where M : MonadIO<M> =>
        producers.Map(p => (p | Conduit.ToConsumerT<M>()).Run())
                 .Traverse(ma => ma.ForkIO());
}
