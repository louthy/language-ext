using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class ConsumerTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ConsumerT`.
    /// </summary>
    public static ConsumerT<IN, M, A> ToConsumer<IN, M, A>(this K<PipeT<IN, Void, M>, A> pipe)
        where M : MonadIO<M> =>
        new(pipe.As());

    /// <summary>
    /// Downcast
    /// </summary>
    public static ConsumerT<IN, M, A> As<IN, M, A>(this K<ConsumerT<IN, M>, A> ma)
        where M : MonadIO<M> =>
        (ConsumerT<IN, M, A>)ma;
    
    /// <summary>
    /// Convert to the `Eff` version of `Consumer`
    /// </summary>
    public static Consumer<RT, IN, A> ToEff<RT, IN, A>(this K<ConsumerT<IN, Eff<RT>>, A> ma) =>
        ma.As();

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ConsumerT.liftM<IN, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this IO<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ConsumerT.liftIO<IN, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ConsumerT.pure<IN, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ConsumerT.lift<IN, M, A>(ff.Function).SelectMany(f, g);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, B> Bind<IN, M, A, B>(
        this K<M, A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B> g)
        where M : MonadIO<M> =>
        ConsumerT.liftM<IN, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, B> Bind<IN, M, A, B>(
        this IO<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B> g)
        where M : MonadIO<M> =>
        ConsumerT.liftIO<IN, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, B> Bind<IN, M, A, B>(
        this Pure<A> ma, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B> g)
        where M : MonadIO<M> =>
        ConsumerT.pure<IN, M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static ConsumerT<IN, M, B> Bind<IN, M, A, B>(
        this Lift<A> ff, 
        Func<A, ConsumerT<IN, M, B>> f,
        Func<A, B> g)
        where M : MonadIO<M> =>
        ConsumerT.lift<IN, M, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, E, M, A, C>(
        this K<ConsumerT<IN, M>, A> ma,
        Func<A, Guard<E, Unit>> bind,
        Func<A, Unit, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => ConsumerT.pure<IN, M, C>(project(a, default)),
                         var guard      => ConsumerT.fail<IN, E, M, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static ConsumerT<IN, M, C> SelectMany<IN, E, M, B, C>(
        this Guard<E, Unit> ma,
        Func<Unit, K<ConsumerT<IN, M>, B>> bind,
        Func<Unit, B, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => ConsumerT.fail<IN, E, M, C>(guard.OnFalse())
        };     

    [Pure]
    public static ConsumerT<IN, M, B> MapIO<IN, M, A, B>(this ConsumerT<IN, M, A> ma, Func<IO<A>, IO<B>> f) 
        where M : MonadUnliftIO<M> =>
        ma.Proxy.MapIO(f);
}
