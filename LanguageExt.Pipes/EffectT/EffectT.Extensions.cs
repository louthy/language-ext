using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class EffectTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `EffectT`.
    /// </summary>
    public static EffectT<M, A> ToEffect<M, A>(this K<PipeT<Unit, Void, M>, A> pipe)
        where M : Monad<M> =>
        new(pipe.As());

    /// <summary>
    /// Downcast
    /// </summary>
    public static EffectT<M, A> As<M, A>(this K<EffectT<M>, A> ma)
        where M : Monad<M> =>
        (EffectT<M, A>)ma;
    
    /// <summary>
    /// Convert to the `Eff` version of `Effect`
    /// </summary>
    public static Effect<RT, A> ToEff<RT, A>(this K<EffectT<Eff<RT>>, A> ma) =>
        ma.As();

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

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, B> Bind<M, A, B>(
        this K<M, A> ma, 
        Func<A, EffectT<M, B>> f)
        where M : Monad<M> =>
        EffectT.liftM(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, B> Bind<M, A, B>(
        this IO<A> ma, 
        Func<A, EffectT<M, B>> f)
        where M : Monad<M> =>
        EffectT.liftIO<M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, B> Bind<M, A, B>(
        this Pure<A> ma, 
        Func<A, EffectT<M, B>> f)
        where M : Monad<M> =>
        EffectT.pure<M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static EffectT<M, B> Bind<M, A, B>(
        this Lift<A> ff, 
        Func<A, EffectT<M, B>> f)
        where M : Monad<M> =>
        EffectT.lift<M, A>(ff.Function).Bind(f);    
}
