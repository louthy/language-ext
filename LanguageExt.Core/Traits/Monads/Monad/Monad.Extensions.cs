using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class MonadExtensions
{
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Monadic bind function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <returns>M〈B〉</returns>
    public static K<M, B> Bind<M, A, B>(
        this K<M, A> ma,
        Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Monadic bind function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <returns>M〈B〉</returns>
    public static K<M, B> Bind<M, A, B>(
        this K<M, A> ma,
        Func<A, Pure<B>> f)
        where M : Functor<M> =>
        M.Map(x => f(x).Value, ma);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma,
        Func<A, K<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        M.Bind(ma, a => M.Map(b => project(a, b) , bind(a)));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma,
        Func<A, Pure<B>> bind,
        Func<A, B, C> project)
        where M : Functor<M> =>
        M.Map(a => project(a, bind(a).Value), ma);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<E, M, A, C>(
        this K<M, A> ma,
        Func<A, Guard<E, Unit>> bind,
        Func<A, Unit, C> project)
        where M : Monad<M>, Fallible<E, M> =>
        M.Bind(ma, a => bind(a) switch
                        {
                            { Flag: true } => M.Pure(project(a, default)),
                            var guard      => M.Fail<C>(guard.OnFalse())
                        });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<E, M, A, C>(
        this K<M, A> ma,
        Func<A, Guard<Fail<E>, Unit>> bind,
        Func<A, Unit, C> project)
        where M : Monad<M>, Fallible<E, M> =>
        M.Bind(ma, a => bind(a) switch
                        {
                            { Flag: true } => M.Pure(project(a, default)),
                            var guard      => M.Fail<C>(guard.OnFalse().Value)
                        });
    
    /// <summary>
    /// Monadic join operation
    /// </summary>
    /// <param name="mma"></param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Joined monad</returns>
    public static K<M, A> Flatten<M, A>(this K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Bind(mma, Prelude.identity);
}
