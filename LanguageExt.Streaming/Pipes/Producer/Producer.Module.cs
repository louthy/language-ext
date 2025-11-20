using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

public static class Producer
{
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> yield<RT, OUT>(OUT value) =>
        PipeT.yield<Eff<RT>, Unit, OUT>(value);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> yieldAll<RT, OUT>(IEnumerable<OUT> values) =>
        PipeT.yieldAll<Eff<RT>, Unit, OUT>(values);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> yieldAll<RT, OUT>(IAsyncEnumerable<OUT> values) =>
        PipeT.yieldAll<Eff<RT>, Unit, OUT>(values);
    
    /// <summary>
    /// Evaluate the `M` monad repeatedly, yielding its bound values downstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> yieldRepeat<RT, OUT>(K<Eff<RT>, OUT> ma) =>
        PipeT.yieldRepeat<Eff<RT>, Unit, OUT>(ma);

    /// <summary>
    /// Evaluate the `IO` monad repeatedly, yielding its bound values downstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> yieldRepeatIO<RT, OUT>(IO<OUT> ma) =>
        PipeT.yieldRepeatIO<Eff<RT>, Unit, OUT>(ma);
    
    /// <summary>
    /// Create a producer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> pure<RT, OUT, A>(A value) =>
        PipeT.pure<Unit, OUT, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> error<RT, OUT, A>(Error value) =>
        PipeT.error<Unit, OUT, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create a producer that yields nothing at all
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> empty<RT, OUT, A>() =>
        PipeT.empty<Unit, OUT, Eff<RT>, A>();
    
    /// <summary>
    /// Create a producer that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> lift<RT, OUT, A>(Func<A> f)  =>
        PipeT.lift<Unit, OUT, Eff<RT>, A>(f);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> liftM<RT, OUT, A>(K<Eff<RT>, A> ma) =>
        PipeT.liftM<Unit, OUT, Eff<RT>, A>(ma);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> liftIO<RT, OUT, A>(IO<A> ma) =>
        PipeT.liftIO<Unit, OUT, Eff<RT>, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> liftT<RT, OUT, A>(Func<Producer<RT, OUT, A>> f) =>
        PipeT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> liftT<RT, OUT, A>(Func<ValueTask<Producer<RT, OUT, A>>> f) =>
        PipeT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> liftT<RT, OUT, A>(ValueTask<Producer<RT, OUT, A>> f) =>
        PipeT.liftT(f.Map(p => p.Proxy));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> repeat<RT, OUT, A>(Producer<RT, OUT, A> ma) =>
        PipeT.repeat(ma.Proxy);
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> repeat<RT, OUT, A>(Schedule schedule, Producer<RT, OUT, A> ma) =>
        PipeT.repeat(schedule, ma.Proxy);

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> repeatM<RT, OUT, A>(K<Eff<RT>, A> ma) =>
        PipeT.repeatM<Unit, OUT, Eff<RT>, A>(ma);

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, A> repeatM<RT, OUT, A>(Schedule schedule, K<Eff<RT>, A> ma) =>
        PipeT.repeatM<Unit, OUT, Eff<RT>, A>(schedule, ma);
                
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> fold<RT, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init,
        Producer<RT, OUT, A> Item) =>
        PipeT.fold(Time, Fold, Init, Item.Proxy);

    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> foldUntil<RT, OUT, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        Producer<RT, OUT, A> Item) =>
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
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> foldUntil<RT, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        Producer<RT, OUT, A> Item) =>
        PipeT.foldUntil(Time, Fold, Pred, Init, Item.Proxy);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> foldWhile<RT, OUT, M, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        Producer<RT, OUT, A> Item) =>
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
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Producer<RT, OUT, Unit> foldWhile<RT, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init,
        Producer<RT, OUT, A> Item) =>
        PipeT.foldWhile(Time, Fold, Pred, Init, Item.Proxy);

    /// <summary>
    /// Merge multiple producers
    /// </summary>
    /// <param name="producers">Producers to merge</param>
    /// <param name="settings">Buffer settings</param>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns>Merged producer</returns>
    public static Producer<RT, OUT, Unit> merge<RT, OUT>(
        params Producer<RT, OUT, Unit>[] producers) =>
        merge(toSeq(producers));
    
    /// <summary>
    /// Merge multiple producers
    /// </summary>
    /// <param name="producers">Producers to merge</param>
    /// <param name="settings">Buffer settings</param>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns>Merged producer</returns>
    public static Producer<RT, OUT, Unit> merge<RT, OUT>(
        Seq<Producer<RT, OUT, Unit>> producers, 
        Buffer<OUT>? settings = null) =>
        ProducerT.merge(producers.Map(p => new ProducerT<OUT, Eff<RT>, Unit>(p.Proxy)), settings);
}
