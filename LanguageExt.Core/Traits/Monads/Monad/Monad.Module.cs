using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static partial class Monad
{
    [Pure]
    public static K<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    [Pure]
    public static K<M, A> flatten<M, A>(K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    [Pure]
    public static K<M, B> bind<M, A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    [Pure]
    public static MB bind<M, MB, A, B>(K<M, A> ma, Func<A, MB> f)
        where MB : K<M, B>
        where M : Monad<M> =>
        (MB)bind(ma, x => f(x));

    /// <summary>
    /// When the predicate evaluates to `true`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> when<M>(K<M, bool> Pred, K<M, Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.when(f, Then));

    /// <summary>
    /// When the predicate evaluates to `true`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> when<M>(K<M, bool> Pred, Pure<Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.when(f, M.Pure(Prelude.unit)));

    /// <summary>
    /// When the predicate evaluates to `false`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> unless<M>(K<M, bool> Pred, K<M, Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.unless(f, Then));

    /// <summary>
    /// When the predicate evaluates to `false`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> unless<M>(K<M, bool> Pred, Pure<Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.unless(f, M.Pure(Prelude.unit)));

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
        Pred.Bind(f => f ? Then : M.LiftIO(Else));

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
        Pred.Bind(f => f ? M.LiftIO(Then) : Else);

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
        Pred.Bind(f => f ? M.LiftIO(Then) : M.LiftIO(Else));

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
        Pred.Bind(f => f ? M.Pure(Then.Value) : M.LiftIO(Else));

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
        Pred.Bind(f => f ? M.LiftIO(Then) : M.Pure(Else.Value));
}
