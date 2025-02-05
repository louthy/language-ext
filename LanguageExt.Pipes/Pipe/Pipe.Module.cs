using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Pipe` streaming producer monad-transformer
/// </summary>
public static class Pipe
{
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> yield<RT, IN, OUT>(OUT value) =>
        PipeT.yield<Eff<RT>, IN, OUT>(value);

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> yieldAll<RT, IN, OUT>(IEnumerable<OUT> values) =>
        PipeT.yieldAll<Eff<RT>, IN, OUT>(values);
    
    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> yieldAll<RT, IN, OUT>(IAsyncEnumerable<OUT> values) =>
        PipeT.yieldAll<Eff<RT>, IN, OUT>(values);
    
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns>Pipe</returns>
    public static Pipe<RT, IN, OUT, IN> awaiting<RT, IN, OUT>() =>
        PipeT.awaiting<Eff<RT>, IN, OUT>();

    /// <summary>
    /// Create a pipe that filters out values that return `false` when applied to a predicate function
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <typeparam name="A">Stream value to consume and produce</typeparam>
    /// <returns>Pipe</returns>
    public static Pipe<RT, A, A, Unit> filter<RT, A>(Func<A, bool> f) =>
        PipeT.filter<Eff<RT>, A>(f);

    /// <summary>
    /// Create a pipe from a mapping function
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns>Pipe</returns>
    public static Pipe<RT, IN, OUT, Unit> map<RT, IN, OUT>(Func<IN, OUT> f) =>
        PipeT.map<Eff<RT>, IN, OUT>(f);

    /// <summary>
    /// Create a pipe from a mapping function
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns>Pipe</returns>
    public static Pipe<RT, IN, OUT, Unit> mapM<RT, IN, OUT>(Func<IN, K<Eff<RT>, OUT>> f) =>
        PipeT.mapM(f);
    
    /// <summary>
    /// Create a pipe that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> pure<RT, IN, OUT, A>(A value) =>
        PipeT.pure<IN, OUT, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create a pipe that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> error<RT, IN, OUT, A>(Error value) =>
        PipeT.error<IN, OUT, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create a pipe that yields nothing at all
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> empty<RT, IN, OUT, A>() => 
        PipeT.empty<IN, OUT, Eff<RT>, A>();
    
    /// <summary>
    /// Create a pipe that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> lift<RT, IN, OUT, A>(Func<A> f) => 
        PipeT.lift<IN, OUT, Eff<RT>, A>(f);
    
    /// <summary>
    /// Create a lazy pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftT<RT, IN, OUT, A>(Func<Pipe<RT, IN, OUT, A>> f) =>
        PipeT.liftT(() => f().Proxy);

    /// <summary>
    /// Create an asynchronous lazy pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftT<RT, IN, OUT, A>(Func<ValueTask<Pipe<RT, IN, OUT, A>>> f) =>
        PipeT.liftT(async () => (await f()).Proxy);

    /// <summary>
    /// Create an asynchronous pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftT<RT, IN, OUT, A>(ValueTask<Pipe<RT, IN, OUT, A>> task) =>
        PipeT.liftT(task.Map(t => t.Proxy));

    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftM<RT, IN, OUT, A>(K<Eff<RT>, A> ma) =>
        PipeT.liftM<IN, OUT, Eff<RT>, A>(ma);

    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftM<RT, IN, OUT, A>(ValueTask<K<Eff<RT>, A>> ma) =>
        PipeT.liftM<IN, OUT, Eff<RT>, A>(ma);
    
    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> liftIO<RT, IN, OUT, A>(IO<A> ma) => 
        PipeT.liftIO<IN, OUT, Eff<RT>, A>(ma);

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> repeat<RT, IN, OUT, A>(Pipe<RT, IN, OUT, A> ma) =>
        PipeT.repeat(ma.Proxy);

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> repeat<RT, IN, OUT, A>(Schedule schedule, Pipe<RT, IN, OUT, A> ma) =>
        PipeT.repeat(schedule, ma.Proxy);

    /// <summary>
    /// Continually lift & repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> repeatM<RT, IN, OUT, A>(K<Eff<RT>, A> ma) =>
        PipeT.repeatM<IN, OUT, Eff<RT>, A>(ma);
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, A> repeatM<RT, IN, OUT, A>(Schedule schedule, K<Eff<RT>, A> ma) =>
        PipeT.repeatM<IN, OUT, Eff<RT>, A>(schedule, ma);
            
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> fold<RT, IN, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init, 
        Pipe<RT, IN, OUT, A> Item) =>
        Item.Proxy.Fold(Time, Fold, Init);
    
    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init, 
        Pipe<RT, IN, OUT, A> Item) =>
        Item.Proxy.FoldUntil(Fold, Pred, Init);

    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold,
        Func<(OUT State, A Value), bool> Pred,
        OUT Init,
        Pipe<RT, IN, OUT, A> Item) =>
        Item.Proxy.FoldUntil(Time, Fold, Pred, Init);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init, 
        Pipe<RT, IN, OUT, A> Item) =>
        Item.Proxy.FoldWhile(Fold, Pred, Init);

    /// <summary>
    /// Fold the given pipe while the predicate is `true` or the `Schedule` completes.
    /// Once `false`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold,
        Func<(OUT State, A Value), bool> Pred,
        OUT Init,
        Pipe<RT, IN, OUT, A> Item) =>
        Item.Proxy.FoldWhile(Time, Fold, Pred, Init);
    
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> fold<RT, IN, OUT>(
        Schedule Time,
        Func<OUT, IN, OUT> Fold, 
        OUT Init) =>
        PipeT.awaiting<Eff<RT>, IN, OUT>().Fold(Time, Fold, Init);
    
    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(
        Func<OUT, IN, OUT> Fold, 
        Func<(OUT State, IN Value), bool> Pred, 
        OUT Init)  =>
        PipeT.awaiting<Eff<RT>, IN, OUT>().FoldUntil(Fold, Pred, Init);
        
    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(
        Schedule Time,
        Func<OUT, IN, OUT> Fold, 
        Func<(OUT State, IN Value), bool> Pred, 
        OUT Init) =>
        PipeT.awaiting<Eff<RT>, IN, OUT>().FoldUntil(Time, Fold, Pred, Init);

    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(
        Func<OUT, IN, OUT> Fold,
        Func<(OUT State, IN Value), bool> Pred,
        OUT Init) =>
        PipeT.awaiting<Eff<RT>, IN, OUT>().FoldWhile(Fold, Pred, Init);

    /// <summary>
    /// Fold the given pipe while the predicate is `true` or the `Schedule` completes.
    /// Once `false`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <returns></returns>
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(
        Schedule Time,
        Func<OUT, IN, OUT> Fold, 
        Func<(OUT State, IN Value), bool> Pred, 
        OUT Init) =>
        PipeT.awaiting<Eff<RT>, IN, OUT>().FoldWhile(Time, Fold, Pred, Init);
}
