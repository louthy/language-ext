using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class EffectTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `EffectT`.
    /// </summary>
    public static EffectT<M, A> ToEffect<M, A>(this PipeT<Unit, Void, M, A> pipe)
        where M : Monad<M> =>
        new(pipe);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, EffectT<M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        EffectT.liftM(ma).SelectMany(f, g);

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
    public static EffectT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma, 
        Func<A, EffectT<M, B>> f,
        Func<A, B, C> g)
        where M : Monad<M> =>
        EffectT.pure<M, A>(ma.Value).SelectMany(f, g);

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
