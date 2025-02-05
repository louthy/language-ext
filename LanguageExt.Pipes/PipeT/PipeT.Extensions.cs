using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public static class PipeTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    public static PipeT<IN, OUT, M, A> As<IN, OUT, M, A>(this K<PipeT<IN, OUT, M>, A> ma)
        where M : Monad<M> =>
        (PipeT<IN, OUT, M, A>)ma;
    
    /// <summary>
    /// Convert to the `Eff` version of `Pipe`
    /// </summary>
    public static Pipe<RT, IN, OUT, A> ToEff<RT, IN, OUT, A>(this K<PipeT<IN, OUT, Eff<RT>>, A> ma) =>
        ma.As();
    
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
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this K<M, A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : Monad<M> =>
        PipeT.liftM<IN, OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this IO<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : Monad<M> =>
        PipeT.liftIO<IN, OUT, M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this Pure<A> ma, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : Monad<M> =>
        PipeT.pure<IN, OUT, M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public static PipeT<IN, OUT, M, B> Bind<IN, OUT, M, A, B>(
        this Lift<A> ff, 
        Func<A, PipeT<IN, OUT, M, B>> f)
        where M : Monad<M> =>
        PipeT.lift<IN, OUT, M, A>(ff.Function).Bind(f);    
}
    
