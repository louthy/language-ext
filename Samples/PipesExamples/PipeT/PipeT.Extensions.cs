using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class PipeTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    public static ProducerT<OUT, M, A> ToProducer<OUT, M, A>(this PipeT<Unit, OUT, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Transformation from `PipeT` to `ConsumerT`.
    /// </summary>
    public static ConsumerT<IN, M, A> ToConsumer<IN, M, A>(this PipeT<IN, Void, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Transformation from `PipeT` to `EffectT`.
    /// </summary>
    public static EffectT<M, A> ToEffect<M, A>(this PipeT<Unit, Void, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this IO<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.liftIO<IN, OUT, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this IO<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.liftIO<OUT, M, A>(ma).SelectMany(f, g);

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
    public static EffectT<M, C> SelectMany<M, A, B, C>(
        this IO<A> ma, 
        Func<A, EffectT<M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        EffectT.liftIO<M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.pure<IN, OUT, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.pure<OUT, M, A>(ma.Value).SelectMany(f, g);

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
    public static EffectT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma, 
        Func<A, EffectT<M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        EffectT.pure<M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.lift<IN, OUT, M, A>(ff.Function).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.lift<OUT, M, A>(ff.Function).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ConsumerT.lift<IN, M, A>(ff.Function).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, C> SelectMany<M, A, B, C>(
        this Lift<A> ff, 
        Func<A, EffectT<M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        EffectT.lift<M, A>(ff.Function).SelectMany(f, g);    
}
    
