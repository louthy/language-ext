using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `ConsumerT` streaming consumer monad-transformer
/// </summary>
public static class ConsumerT
{
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, IN> awaiting<M, IN>()
        where M : Monad<M> =>
        PipeT.awaiting<M, IN, Void>();

    /// <summary>
    /// Await a value from upstream and then ignore it
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, Unit> awaitIgnore<M, IN>()
        where M : Monad<M> =>
        new PipeTAwait<IN, Void, M, Unit>(_ => PipeT.pure<IN, Void, M, Unit>(default));

    /// <summary>
    /// Create a consumer that simply returns a bound value without awaiting anything
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> pure<IN, M, A>(A value)
        where M : Monad<M> =>
        PipeT.pure<IN, Void, M, A>(value);

    /// <summary>
    /// Create a consumer that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> fail<IN, E, M, A>(E value)
        where M : Monad<M>, Fallible<E, M> =>
        PipeT.fail<IN, Void, E, M, A>(value);

    /// <summary>
    /// Create a consumer that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> error<IN, M, A>(Error value)
        where M : Monad<M>, Fallible<M> =>
        PipeT.fail<IN, Void, Error, M, A>(value);

    /// <summary>
    /// Create a consumer that yields nothing at all
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> empty<IN, M, A>()
        where M : Monad<M>, MonoidK<M> =>
        PipeT.empty<IN, Void, M, A>();

    /// <summary>
    /// Create a consumer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> lift<IN, M, A>(Func<A> f)
        where M : Monad<M> =>
        PipeT.lift<IN, Void, M, A>(f);

    /// <summary>
    /// Create a lazy consumer 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftT<IN, M, A>(Func<ConsumerT<IN, M, A>> f)
        where M : Monad<M> =>
        PipeT.liftT(() => f().Proxy);

    /// <summary>
    /// Create an asynchronous lazy consumer 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftT<IN, M, A>(Func<ValueTask<ConsumerT<IN, M, A>>> f)
        where M : Monad<M> =>
        PipeT.liftT(() => f().Map(x => x.Proxy));

    /// <summary>
    /// Create an asynchronous consumer 
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftT<IN, M, A>(ValueTask<ConsumerT<IN, M, A>> f)
        where M : Monad<M> =>
        PipeT.liftT(f.Map(x => x.Proxy));

    /// <summary>
    /// Create a consumer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftM<IN, M, A>(K<M, A> ma)
        where M : Monad<M> =>
        PipeT.liftM<IN, Void, M, A>(ma);

    /// <summary>
    /// Create a consumer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftIO<IN, M, A>(IO<A> ma)
        where M : Monad<M> =>
        PipeT.liftIO<IN, Void, M, A>(ma);

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> repeat<IN, M, A>(ConsumerT<IN, M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(ma.Proxy).ToConsumer();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> repeat<IN, M, A>(Schedule schedule, ConsumerT<IN, M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(schedule, ma.Proxy).ToConsumer();

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> repeatM<IN, M, A>(K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<IN, Void, M, A>(ma).ToConsumer();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> repeatM<IN, M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<IN, Void, M, A>(schedule, ma).ToConsumer();
}
