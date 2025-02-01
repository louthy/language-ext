using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class ConsumerTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ConsumerT`.
    /// </summary>
    public static ConsumerT<IN, M, A> ToConsumer<IN, M, A>(this K<PipeT<IN, Void, M>, A> pipe)
        where M : Monad<M> =>
        new(pipe.As());

    /// <summary>
    /// Downcast
    /// </summary>
    public static ConsumerT<IN, M, A> As<IN, M, A>(this K<ConsumerT<IN, M>, A> ma)
        where M : Monad<M> =>
        (ConsumerT<IN, M, A>)ma;

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ConsumerT.liftM<IN, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this IO<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ConsumerT.liftIO<IN, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ConsumerT.pure<IN, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ConsumerT.lift<IN, M, A>(ff.Function).SelectMany(f, g);
    
}
