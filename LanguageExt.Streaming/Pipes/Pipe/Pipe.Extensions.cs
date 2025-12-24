using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class PipeExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static Pipe<RT, IN, OUT, A> As<RT, IN, OUT, A>(this K<Pipe<RT, IN, OUT>, A> ma) =>
        (Pipe<RT, IN, OUT, A>)ma;
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(
        this K<Eff<RT>, A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f,
        Func<A, B, C> g) =>
        Pipe.liftM<RT, IN, OUT, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(
        this IO<A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f,
        Func<A, B, C> g) =>
        Pipe.liftIO<RT, IN, OUT, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(
        this Pure<A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f,
        Func<A, B, C> g) =>
        Pipe.pure<RT, IN, OUT, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(
        this Lift<A> ff, 
        Func<A, Pipe<RT, IN, OUT, B>> f,
        Func<A, B, C> g) =>
        Pipe.lift<RT, IN, OUT, A>(ff.Function).SelectMany(f, g);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, A, B>(
        this K<Eff<RT>, A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f) =>
        Pipe.liftM<RT, IN, OUT, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, A, B>(
        this IO<A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f) =>
        Pipe.liftIO<RT, IN, OUT, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, A, B>(
        this Pure<A> ma, 
        Func<A, Pipe<RT, IN, OUT, B>> f) =>
        Pipe.pure<RT, IN, OUT, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, A, B>(
        this Lift<A> ff, 
        Func<A, Pipe<RT, IN, OUT, B>> f) =>
        Pipe.lift<RT, IN, OUT, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, C>(
        this K<Pipe<RT, IN, OUT>, A> ma,
        Func<A, Guard<Error, Unit>> bind,
        Func<A, Unit, C> project) =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => Pipe.pure<RT, IN, OUT, C>(project(a, default)),
                         var guard      => Pipe.error<RT, IN, OUT, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, B, C>(
        this Guard<Error, Unit> ma,
        Func<Unit, K<Pipe<RT, IN, OUT>, B>> bind,
        Func<Unit, B, C> project) =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => Pipe.error<RT, IN, OUT, C>(guard.OnFalse())
        };     
}
    
