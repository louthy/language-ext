using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class PipeTExtensions
{
    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.liftM<IN, OUT, M, A>(ma).SelectMany(f, g);

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
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.pure<IN, OUT, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        PipeT.lift<IN, OUT, M, A>(ff.Function).SelectMany(f, g);
}
    
