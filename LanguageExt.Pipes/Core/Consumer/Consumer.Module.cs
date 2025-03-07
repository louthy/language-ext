using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `ConsumerT` streaming consumer monad-transformer
/// </summary>
public static class Consumer
{
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, IN> awaiting<RT, IN>() =>
        PipeT.awaiting<Eff<RT>, IN, Void>().ToConsumer();

    /// <summary>
    /// Await a value from upstream and then ignore it
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, Unit> awaitIgnore<RT, IN>() =>
        new PipeTAwait<IN, Void, Eff<RT>, Unit>(_ => PipeT.pure<IN, Void, Eff<RT>, Unit>(default)).ToConsumer();

    /// <summary>
    /// Create a consumer that simply returns a bound value without awaiting anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> pure<RT, IN, A>(A value) =>
        PipeT.pure<IN, Void, Eff<RT>, A>(value).ToConsumer();

    /// <summary>
    /// Create a consumer that always fails
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> error<RT, IN, A>(Error value) =>
        PipeT.fail<IN, Void, Error, Eff<RT>, A>(value).ToConsumer();

    /// <summary>
    /// Create a consumer that yields nothing at all
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> empty<RT, IN, A>() =>
        PipeT.empty<IN, Void, Eff<RT>, A>().ToConsumer();

    /// <summary>
    /// Create a consumer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> lift<RT, IN, A>(Func<A> f) =>
        PipeT.lift<IN, Void, Eff<RT>, A>(f).ToConsumer();

    /// <summary>
    /// Create a lazy consumer 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> liftT<RT, IN, A>(Func<Consumer<RT, IN, A>> f) =>
        PipeT.liftT(() => f().Proxy).ToConsumer();

    /// <summary>
    /// Create an asynchronous lazy consumer 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> liftT<RT, IN, A>(Func<ValueTask<Consumer<RT, IN, A>>> f) =>
        PipeT.liftT(() => f().Map(x => x.Proxy)).ToConsumer();

    /// <summary>
    /// Create an asynchronous consumer 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> liftT<RT, IN, A>(ValueTask<Consumer<RT, IN, A>> f) =>
        PipeT.liftT(f.Map(x => x.Proxy)).ToConsumer();

    /// <summary>
    /// Create a consumer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> liftM<RT, IN, A>(K<Eff<RT>, A> ma) =>
        PipeT.liftM<IN, Void, Eff<RT>, A>(ma).ToConsumer();

    /// <summary>
    /// Create a consumer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> liftIO<RT, IN, A>(IO<A> ma) =>
        PipeT.liftIO<IN, Void, Eff<RT>, A>(ma).ToConsumer();

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> repeat<RT, IN, A>(Consumer<RT, IN, A> ma) =>
        PipeT.repeat(ma.Proxy).ToConsumer();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> repeat<RT, IN, A>(Schedule schedule, Consumer<RT, IN, A> ma) =>
        PipeT.repeat(schedule, ma.Proxy).ToConsumer();

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> repeatM<RT, IN, A>(K<Eff<RT>, A> ma) =>
        PipeT.repeatM<IN, Void, Eff<RT>, A>(ma).ToConsumer();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Consumer<RT, IN, A> repeatM<RT, IN, A>(Schedule schedule, K<Eff<RT>, A> ma) =>
        PipeT.repeatM<IN, Void, Eff<RT>, A>(schedule, ma).ToConsumer();
}
