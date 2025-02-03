using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes2;

/// <summary>
/// `PipeT` streaming producer monad-transformer
/// </summary>
public static class PipeT
{
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yield<M, IN, OUT>(OUT value) 
        where M : Monad<M> =>
        new PipeTYield<IN, OUT, M, Unit>(value, _ => pure<IN, OUT, M, Unit>(default));

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yieldAll<M, IN, OUT>(IEnumerable<OUT> values)
        where M : Monad<M> =>
        new PipeTYieldAll<IN, OUT, M, Unit>(values.Select(yield<M, IN, OUT>), pure<IN, OUT, M, Unit>);
    
    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yieldAll<M, IN, OUT>(IAsyncEnumerable<OUT> values) 
        where M : Monad<M> =>
        new PipeTYieldAllAsync<IN, OUT, M, Unit>(values.Select(yield<M, IN, OUT>), pure<IN, OUT, M, Unit>);
    
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, IN> awaiting<M, IN, OUT>() 
        where M : Monad<M> =>
        new PipeTAwait<IN, OUT, M, IN>(pure<IN, OUT, M, IN>);
    
    /// <summary>
    /// Create a pipe that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> pure<IN, OUT, M, A>(A value) 
        where M : Monad<M> =>
        new PipeTPure<IN, OUT, M, A>(value);
    
    /// <summary>
    /// Create a pipe that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> fail<IN, OUT, E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        new PipeTFail<IN, OUT, E, M, A>(value);
    
    /// <summary>
    /// Create a pipe that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> error<IN, OUT, M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        new PipeTFail<IN, OUT, Error, M, A>(value);
    
    /// <summary>
    /// Create a pipe that yields nothing at all
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> empty<IN, OUT, M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        PipeTEmpty<IN, OUT, M, A>.Default;
    
    /// <summary>
    /// Create a pipe that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> lift<IN, OUT, M, A>(Func<A> f) 
        where M : Monad<M> =>
        new PipeTLazy<IN, OUT, M, A>(() => pure<IN, OUT, M, A>(f()));
    
    /// <summary>
    /// Create a lazy pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftT<IN, OUT, M, A>(Func<PipeT<IN, OUT, M, A>> f) 
        where M : Monad<M> =>
        new PipeTLazy<IN, OUT, M, A>(f);
    
    /// <summary>
    /// Create an asynchronous lazy pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftT<IN, OUT, M, A>(Func<ValueTask<PipeT<IN, OUT, M, A>>> f) 
        where M : Monad<M> =>
        new PipeTLazyAsync<IN, OUT, M, A>(f);

    /// <summary>
    /// Create an asynchronous pipe 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftT<IN, OUT, M, A>(ValueTask<PipeT<IN, OUT, M, A>> task)
        where M : Monad<M> =>
        new PipeTLazyAsync<IN, OUT, M, A>(() => task);
    
    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftM<IN, OUT, M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        new PipeTLiftM<IN, OUT, M, A>(ma.Map(pure<IN, OUT, M, A>));
    
    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftM<IN, OUT, M, A>(ValueTask<K<M, A>> ma) 
        where M : Monad<M> =>
        new PipeTLazyAsync<IN, OUT, M, A>(() => ma.Map(liftM<IN, OUT, M, A>));
    
    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftIO<IN, OUT, M, A>(IO<A> ma) 
        where M : Monad<M> =>
        liftM<IN, OUT, M, A>(M.LiftIO(ma));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> repeat<IN, OUT, M, A>(PipeT<IN, OUT, M, A> ma)
        where M : Monad<M> =>
        ma.Bind(_ => repeat(ma));

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> repeat<IN, OUT, M, A>(Schedule schedule, PipeT<IN, OUT, M, A> ma)
        where M : Monad<M>
    {
        return from s in pure<IN, OUT, M, Iterator<Duration>>(schedule.Run().GetIterator())
               from r in ma
               from t in go(s, ma, r)
               select t;

        static PipeT<IN, OUT, M, A> go(Iterator<Duration> schedule, PipeT<IN, OUT, M, A> ma, A latest) =>
            schedule.IsEmpty
                ? pure<IN, OUT, M, A>(latest)
                : liftIO<IN, OUT, M, Unit>(IO.yieldFor(schedule.Head))
                   .Bind(_ => ma.Bind(x => go(schedule.Tail, ma, x))); 
    }

    /// <summary>
    /// Continually lift & repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> repeatM<IN, OUT, M, A>(K<M, A> ma)
        where M : Monad<M> =>
        repeat(liftM<IN, OUT, M, A>(ma));

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> repeatM<IN, OUT, M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        repeat(schedule, liftM<IN, OUT, M, A>(ma));
    
    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial value</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> foldUntil<IN, OUT, M, A>(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init, 
        PipeT<IN, OUT, M, A> Item)
        where M : Monad<M>
    {
        var state = Init;
        return Item.Bind(
            x =>
            {
                if (Pred((state, x)))
                {
                    var nstate = state;
                    state = Init;
                    return yield<M, IN, OUT>(nstate);
                }
                else
                {
                    state = Fold(state, x);
                    return PipeTCached<IN, OUT, M>.unitP;
                }
            });
    }
        
    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial value</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> foldUntil<IN, OUT, M, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init, 
        PipeT<IN, OUT, M, A> Item)
        where M : Monad<M>
    {
        var state = Init;
        var sch   = Time.Run().GetIterator();
        return Item.Bind(
            x =>
            {
                if (sch.IsEmpty || Pred((state, x)))
                {
                    sch.Dispose();
                    sch = Time.Run().GetIterator();
                    var nstate = state;
                    state = Init;
                    return yield<M, IN, OUT>(nstate);
                }
                else
                {
                    state = Fold(state, x);
                    var delay = sch.Head;
                    sch = sch.Tail;
                    return liftIO<IN, OUT, M, Unit>(IO.yieldFor(delay));
                }
            });
    }
            
    /// <summary>
    /// Fold the given pipe until the predicate the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial value</param>
    /// <param name="Item">Pipe to fold</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> foldUntil<IN, OUT, M, A>(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init, 
        PipeT<IN, OUT, M, A> Item)
        where M : Monad<M>
    {
        var state = Init;
        var sch   = Time.Run().GetIterator();
        return Item.Bind(
            x =>
            {
                if (sch.IsEmpty)
                {
                    sch.Dispose();
                    sch = Time.Run().GetIterator();
                    var nstate = state;
                    state = Init;
                    return yield<M, IN, OUT>(nstate);
                }
                else
                {
                    state = Fold(state, x);
                    var delay = sch.Head;
                    sch = sch.Tail;
                    return liftIO<IN, OUT, M, Unit>(IO.yieldFor(delay));
                }
            });
    }
}
