using LanguageExt.Common;
using LanguageExt.Traits;

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
}
