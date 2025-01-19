using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `PipeT` streaming producer monad-transformer
/// </summary>
public static class ProxyT
{
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
    public static PipeT<IN, OUT, M, A> lift<IN, OUT, M, A>(Func<A> value) 
        where M : Monad<M> =>
        new PipeTLift<IN, OUT, A, M, A>(value, pure<IN, OUT, M, A>);
    
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
    public static PipeT<IN, OUT, M, A> liftT<IN, OUT, M, A>(ValueTask<PipeT<IN, OUT, M, A>> f) 
        where M : Monad<M> =>
        new PipeTAsync<IN, OUT, M, A>(f);
    
    /// <summary>
    /// Create a pipe that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, A> liftM<IN, OUT, M, A>(K<M, A> value) 
        where M : Monad<M> =>
        new PipeTLiftM<IN, OUT, M, A>(value.Map(pure<IN, OUT, M, A>));
    
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yield<IN, OUT, M>(OUT value) 
        where M : Monad<M> =>
        new PipeTYield<IN, OUT, M, Unit>(value, _ => pure<IN, OUT, M, Unit>(default));

    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yieldAll<IN, OUT, M>(IEnumerable<OUT> values)
        where M : Monad<M>, Alternative<M> =>
        new PipeTYieldAll<IN, OUT, M, Unit>(
            values.Select(v => new PipeTYield<IN, OUT, M, Unit>(v, _ => pure<IN, OUT, M, Unit>(default))));
    
    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, Unit> yieldAll<IN, OUT, M>(IAsyncEnumerable<OUT> values) 
        where M : Monad<M>, Alternative<M> =>
        new PipeTYieldAllAsync<IN, OUT, M, Unit>(
            values.Select(v => new PipeTYield<IN, OUT, M, Unit>(v, _ => pure<IN, OUT, M, Unit>(default))));
    
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static PipeT<IN, OUT, M, IN> awaiting<IN, OUT, M>() 
        where M : Monad<M> =>
        new PipeTAwait<IN, OUT, M, IN>(pure<IN, OUT, M, IN>);
}
