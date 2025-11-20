using System;
using LanguageExt.Traits;

namespace LanguageExt.ContT;

/// <summary>
/// The continuation monad transformer.
/// </summary>
/// <remarks>
/// Can be used to add continuation handling to any type constructor:
/// the `Monad` trait-implementation and most of the operations do not
/// require `M` to be a monad.
/// </remarks>
/// <param name="runCont">The continuation</param>
/// <typeparam name="R"></typeparam>
/// <typeparam name="M"></typeparam>
/// <typeparam name="A"></typeparam>
public record ContT<R, M, A>(Func<Func<A, K<M, R>>, K<M, R>> runCont)
    where M : Applicative<M>
{
    /// <summary>
    /// Monadic bind operation
    /// </summary>
    /// <param name="f">Bind function</param>
    public ContT<R, M, B> Map<B>(Func<A, B> f) =>
        new(c => runCont(x => c(f(x))));

    /// <summary>
    /// Monadic bind operation
    /// </summary>
    /// <param name="f">Bind function</param>
    public ContT<R, M, B> Bind<B>(Func<A, ContT<R, M, B>> f) =>
        new(c => runCont(x => f(x).runCont(c)));

    /// <summary>
    /// Run the continuation, passing the final continuation
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public K<M, R> Run(Func<A, R> f) =>
        runCont(x => M.Pure(f(x)));
}
