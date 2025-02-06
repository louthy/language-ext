using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class EffectExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `EffectT`.
    /// </summary>
    public static Effect<RT, A> ToEffect<RT, A>(this K<PipeT<Unit, Void, Eff<RT>>, A> pipe) =>
        new(pipe.As());

    /// <summary>
    /// Downcast
    /// </summary>
    public static Effect<RT, A> As<RT, A>(this K<Effect<RT>, A> ma) =>
        (Effect<RT, A>)ma;

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, A, B, C>(
        this K<Eff<RT>, A> ma, 
        Func<A, Effect<RT, B>> f,
        Func<A, B, C> g) =>
        Effect.liftM(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, A, B, C>(
        this IO<A> ma, 
        Func<A, Effect<RT, B>> f,
        Func<A, B, C> g) =>
        Effect.liftIO<RT, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, A, B, C>(
        this Pure<A> ma, 
        Func<A, Effect<RT, B>> f,
        Func<A, B, C> g) =>
        Effect.pure<RT, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, A, B, C>(
        this Lift<A> ff, 
        Func<A, Effect<RT, B>> f,
        Func<A, B, C> g) =>
        Effect.lift<RT, A>(ff.Function).SelectMany(f, g);    

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, B> Bind<RT, A, B>(
        this K<Eff<RT>, A> ma, 
        Func<A, Effect<RT, B>> f) =>
        Effect.liftM(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, B> Bind<RT, A, B>(
        this IO<A> ma, 
        Func<A, Effect<RT, B>> f) =>
        Effect.liftIO<RT, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, B> Bind<RT, A, B>(
        this Pure<A> ma, 
        Func<A, Effect<RT, B>> f) =>
        Effect.pure<RT, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Effect<RT, B> Bind<RT, A, B>(
        this Lift<A> ff, 
        Func<A, Effect<RT, B>> f) =>
        Effect.lift<RT, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, A, C>(
        this K<Effect<RT>, A> ma,
        Func<A, Guard<Error, Unit>> bind,
        Func<A, Unit, C> project) =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => Effect.pure<RT, C>(project(a, default)),
                         var guard      => Effect.error<RT, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Effect<RT, C> SelectMany<RT, B, C>(
        this Guard<Error, Unit> ma,
        Func<Unit, K<Effect<RT>, B>> bind,
        Func<Unit, B, C> project) =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => Effect.error<RT, C>(guard.OnFalse())
        };    
}
