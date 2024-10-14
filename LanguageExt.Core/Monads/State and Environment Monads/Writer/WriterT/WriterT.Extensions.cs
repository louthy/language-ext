using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    public static WriterT<W, M, A> As<W, M, A>(this K<WriterT<W, M>, A> ma)
        where M : Monad<M>, SemigroupK<M> 
        where W : Monoid<W> =>
        (WriterT<W, M, A>)ma;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static WriterT<W, M, A> Flatten<W, M, A>(this WriterT<W, M, WriterT<W, M, A>> mma)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        mma.Bind(x => x);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public static WriterT<W, M, C> SelectMany<W, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<WriterT<W, M>, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        WriterT<W, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`StateT`</returns>
    public static WriterT<W, M, C> SelectMany<W, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, WriterT<W, M, B>> bind, 
        Func<A, B, C> project)
        where W : Monoid<W>
        where M : Monad<M>, SemigroupK<M> =>
        WriterT<W, M, A>.Lift(ma).SelectMany(bind, project);
}
