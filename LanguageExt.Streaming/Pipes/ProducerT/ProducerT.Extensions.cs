using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class ProducerTExtensions
{
    /// <summary>
    /// Transformation from `PipeT` to `ProducerT`.
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, A> ToProducer<OUT, M, A>(this K<PipeT<Unit, OUT, M>, A> pipe)
        where M : MonadIO<M> =>
        new(pipe.As());
    
    /// <summary>
    /// Downcast
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, A> As<OUT, M, A>(this K<ProducerT<OUT, M>, A> ma)
        where M : MonadIO<M> =>
        (ProducerT<OUT, M, A>)ma;
    
    /// <summary>
    /// Convert to the `Eff` version of `Producer`
    /// </summary>
    [Pure]
    public static Producer<RT, OUT, A> ToEff<RT, OUT, A>(this K<ProducerT<OUT, Eff<RT>>, A> ma) =>
        ma.As();

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ProducerT.liftM<OUT, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this IO<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ProducerT.liftIO<OUT, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ProducerT.pure<OUT, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, ProducerT<OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        ProducerT.lift<OUT, M, A>(ff.Function).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, B> Bind<OUT, M, A, B>(
        this K<M, A> ma, 
        Func<A, ProducerT<OUT, M, B>> f)
        where M : MonadIO<M> =>
        ProducerT.liftM<OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, B> Bind<OUT, M, A, B>(
        this IO<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f)
        where M : MonadIO<M> =>
        ProducerT.liftIO<OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, B> Bind<OUT, M, A, B>(
        this Pure<A> ma, 
        Func<A, ProducerT<OUT, M, B>> f)
        where M : MonadIO<M> =>
        ProducerT.pure<OUT, M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, B> Bind<OUT, M, A, B>(
        this Lift<A> ff, 
        Func<A, ProducerT<OUT, M, B>> f)
        where M : MonadIO<M> =>
        ProducerT.lift<OUT, M, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, E, M, A, C>(
        this K<ProducerT<OUT, M>, A> ma,
        Func<A, Guard<E, Unit>> bind,
        Func<A, Unit, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => ProducerT.pure<OUT, M, C>(project(a, default)),
                         var guard      => ProducerT.fail<OUT, E, M, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static ProducerT<OUT, M, C> SelectMany<OUT, E, M, B, C>(
        this Guard<E, Unit> ma,
        Func<Unit, K<ProducerT<OUT, M>, B>> bind,
        Func<Unit, B, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => ProducerT.fail<OUT, E, M, C>(guard.OnFalse())
        };

    [Pure]
    public static ProducerT<OUT, M, B> MapIO<OUT, M, A, B>(
        this K<ProducerT<OUT, M>, A> ma,
        Func<IO<A>, IO<B>> f) 
        where M : MonadUnliftIO<M> =>
        ma.As().MapM(m => M.MapIOMaybe(m, f));
}
