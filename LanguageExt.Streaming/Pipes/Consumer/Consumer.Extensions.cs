using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ConsumerExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `Consumer`.
    /// </summary>
    public static Consumer<RT, IN, A> ToConsumer<RT, IN, A>(this K<PipeT<IN, Void, Eff<RT>>, A> pipe) =>
        new(pipe.As());
    
    /// <summary>
    /// Transformation from `Pipe` to `Consumer`.
    /// </summary>
    public static Consumer<RT, IN, A> ToConsumer<RT, IN, A>(this K<Pipe<RT, IN, Void>, A> pipe) =>
        new(pipe.As().Proxy);

    /// <summary>
    /// Downcast
    /// </summary>
    public static Consumer<RT, IN, A> As<RT, IN, A>(this K<Consumer<RT, IN>, A> ma) =>
        (Consumer<RT, IN, A>)ma;

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(
        this K<Eff<RT>, A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B, C> g) =>
        Consumer.liftM<RT, IN, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(
        this IO<A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B, C> g) =>
        Consumer.liftIO<RT, IN, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(
        this Pure<A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B, C> g) =>
        Consumer.pure<RT, IN, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(
        this Lift<A> ff, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B, C> g) =>
        Consumer.lift<RT, IN, A>(ff.Function).SelectMany(f, g);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, B> Bind<RT, IN, A, B>(
        this K<Eff<RT>, A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B> g) =>
        Consumer.liftM<RT, IN, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, B> Bind<RT, IN, A, B>(
        this IO<A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B> g) =>
        Consumer.liftIO<RT, IN, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, B> Bind<RT, IN, A, B>(
        this Pure<A> ma, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B> g) =>
        Consumer.pure<RT, IN, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static Consumer<RT, IN, B> Bind<RT, IN, A, B>(
        this Lift<A> ff, 
        Func<A, Consumer<RT, IN, B>> f,
        Func<A, B> g) =>
        Consumer.lift<RT, IN, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, A, C>(
        this K<Consumer<RT, IN>, A> ma,
        Func<A, Guard<Error, Unit>> bind,
        Func<A, Unit, C> project) =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => Consumer.pure<RT, IN, C>(project(a, default)),
                         var guard      => Consumer.error<RT, IN, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static Consumer<RT, IN, C> SelectMany<RT, IN, B, C>(
        this Guard<Error, Unit> ma,
        Func<Unit, K<Consumer<RT, IN>, B>> bind,
        Func<Unit, B, C> project) =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => Consumer.error<RT, IN, C>(guard.OnFalse())
        };    
}
