using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : M.Pure(Else.Value));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : M.Pure(Else.Value));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : M.Pure(Else.Value));

    /// <summary>
    /// If this then that for higher-kinds
    /// </summary>
    /// <param name="Pred">Boolean flag to be computed</param>
    /// <param name="Then">Then branch if the flag computes to `true`</param>
    /// <param name="Else">Else branch if the flag computes to `false`</param>
    /// <typeparam name="F">Trait type</typeparam>
    /// <typeparam name="A">Bound return value</typeparam>
    /// <returns>Returns either the `then` or `else` branches depending on the computed `flag`</returns>
    public static K<F, A> iff<F, A>(bool Pred, K<F, A> Then, K<F, A> Else) =>
        Pred ? Then : Else;
}
