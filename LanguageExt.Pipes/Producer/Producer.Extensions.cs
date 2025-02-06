using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class ProducerExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    public static Producer<RT, OUT, A> ToProducer<RT, OUT, A>(this K<PipeT<Unit, OUT, Eff<RT>>, A> pipe) =>
        new(pipe.As());
    
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    public static Producer<RT, OUT, A> ToProducer<RT, OUT, A>(this K<Pipe<RT, Unit, OUT>, A> pipe) =>
        new(pipe.As().Proxy);
    
    /// <summary>
    /// Downcast
    /// </summary>
    public static Producer<RT, OUT, A> As<RT, OUT, A>(this K<Producer<RT, OUT>, A> ma) =>
        (Producer<RT, OUT, A>)ma;

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(
        this K<Eff<RT>, A> ma, 
        Func<A, Producer<RT, OUT, B>> f,
        Func<A, B, C> g) =>
        Producer.liftM<RT, OUT, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(
        this IO<A> ma, 
        Func<A, Producer<RT, OUT, B>> f,
        Func<A, B, C> g) =>
        Producer.liftIO<RT, OUT, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(
        this Pure<A> ma, 
        Func<A, Producer<RT, OUT, B>> f,
        Func<A, B, C> g) =>
        Producer.pure<RT, OUT, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(
        this Lift<A> ff, 
        Func<A, Producer<RT, OUT, B>> f,
        Func<A, B, C> g) =>
        Producer.lift<RT, OUT, A>(ff.Function).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, B> Bind<RT, OUT, A, B>(
        this K<Eff<RT>, A> ma, 
        Func<A, Producer<RT, OUT, B>> f) =>
        Producer.liftM<RT, OUT, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, B> Bind<RT, OUT, A, B>(
        this IO<A> ma, 
        Func<A, Producer<RT, OUT, B>> f) =>
        Producer.liftIO<RT, OUT, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, B> Bind<RT, OUT, A, B>(
        this Pure<A> ma, 
        Func<A, Producer<RT, OUT, B>> f) =>
        Producer.pure<RT, OUT, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Producer<RT, OUT, B> Bind<RT, OUT, A, B>(
        this Lift<A> ff, 
        Func<A, Producer<RT, OUT, B>> f) =>
        Producer.lift<RT, OUT, A>(ff.Function).Bind(f);
    

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, C>(
        this K<Producer<RT, OUT>, A> ma,
        Func<A, Guard<Error, Unit>> bind,
        Func<A, Unit, C> project) =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => Producer.pure<RT, OUT, C>(project(a, default)),
                         var guard      => Producer.error<RT, OUT, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Producer<RT, OUT, C> SelectMany<RT, OUT, B, C>(
        this Guard<Error, Unit> ma,
        Func<Unit, K<Producer<RT, OUT>, B>> bind,
        Func<Unit, B, C> project) =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => Producer.error<RT, OUT, C>(guard.OnFalse())
        };    
}
