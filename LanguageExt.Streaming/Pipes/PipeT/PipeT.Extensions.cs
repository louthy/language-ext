using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static partial class PipeTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, A> As<IN, OUT, M, A>(this K<PipeT<IN, OUT, M>, A> ma)
        where M : MonadIO<M> =>
        (PipeT<IN, OUT, M, A>)ma;
    
    /// <summary>
    /// Convert to the `Eff` version of `Pipe`
    /// </summary>
    [Pure]
    public static Pipe<RT, IN, OUT, A> ToEff<RT, IN, OUT, A>(this K<PipeT<IN, OUT, Eff<RT>>, A> ma) =>
        ma.As();
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        PipeT.liftM<IN, OUT, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this IO<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        PipeT.liftIO<IN, OUT, M, A>(ma).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Pure<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        PipeT.pure<IN, OUT, M, A>(ma.Value).SelectMany(f, g);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, M, A, B, C>(
        this Lift<A> ff, 
        Func<A, PipeT<IN, OUT, M, B>> f,
        Func<A, B, C> g)
        where M : MonadIO<M> =>
        PipeT.lift<IN, OUT, M, A>(ff.Function).SelectMany(f, g);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this K<M, A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : MonadIO<M> =>
        PipeT.liftM<IN, OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this IO<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : MonadIO<M> =>
        PipeT.liftIO<IN, OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this Pure<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : MonadIO<M> =>
        PipeT.pure<IN, OUT, M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this Lift<A> ff, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : MonadIO<M> =>
        PipeT.lift<IN, OUT, M, A>(ff.Function).Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, E, M, A, C>(
        this K<PipeT<IN, OUT, M>, A> ma,
        Func<A, Guard<E, Unit>> bind,
        Func<A, Unit, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma.Bind(a => bind(a) switch
                     {
                         { Flag: true } => PipeT.pure<IN, OUT, M, C>(project(a, default)),
                         var guard      => PipeT.fail<IN, OUT, E, M, C>(guard.OnFalse())
                     }).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static PipeT<IN, OUT, M, C> SelectMany<IN, OUT, E, M, B, C>(
        this Guard<E, Unit> ma,
        Func<Unit, K<PipeT<IN, OUT, M>, B>> bind,
        Func<Unit, B, C> project)
        where M : MonadIO<M>, Fallible<E, M> =>
        ma switch
        {
            { Flag: true } => bind(default).Map(b => project(default, b)).As(),
            var guard      => PipeT.fail<IN, OUT, E, M, C>(guard.OnFalse())
        };    
        
    [Pure]
    public static PipeT<IN, OUT, M, B> MapIO<IN, OUT, M, A, B>(
        this K<PipeT<IN, OUT, M>, A> ma,
        Func<IO<A>, IO<B>> f) 
        where M : MonadUnliftIO<M> =>
        ma.As().MapM(ma => M.MapIOMaybe(ma, f));
}
    
