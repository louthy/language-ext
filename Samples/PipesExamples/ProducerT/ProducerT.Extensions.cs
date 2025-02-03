using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class ProducerTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    public static ProducerT<OUT, M, A> ToProducer<OUT, M, A>(this K<PipeT<Unit, OUT, M>, A> pipe)
        where M : Monad<M> =>
        new(pipe.As());
    
    /// <summary>
    /// Downcast
    /// </summary>
    public static ProducerT<OUT, M, A> As<OUT, M, A>(this K<ProducerT<OUT, M>, A> ma)
        where M : Monad<M> =>
        (ProducerT<OUT, M, A>)ma;

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.liftM<OUT, M, A>(ma).SelectMany(f, g);

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
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.pure<OUT, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        ProducerT.lift<OUT, M, A>(ff.Function).SelectMany(f, g);
}
