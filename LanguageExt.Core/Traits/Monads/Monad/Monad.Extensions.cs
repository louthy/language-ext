using System;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static partial class Monad
{
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Monadic bind function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <returns>M<B></returns>
    public static MB Bind<MB, M, A, B>(
        this K<M, A> ma,
        Func<A, MB> f)
        where M : Monad<M>
        where MB : K<M, B> =>
        (MB)M.Bind(ma, a => f(a));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M<C></returns>
    public static K<M, C> SelectMany<MB, M, A, B, C>(
        this K<M, A> ma,
        Func<A, MB> bind,
        Func<A, B, C> project)
        where M : Monad<M>
        where MB : K<M, B> =>
        M.Bind(ma, a => M.Map(b => project(a, b) , bind(a)));
}
