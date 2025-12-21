using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

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
    /// Allow for tail-recursion by using a trampoline.  Returns a monad with the bound value wrapped
    /// by `Next`, which enables decision-making about whether to keep the computation going or not.  
    /// </summary>
    /// <remarks>
    /// It is expected that the implementor of the `Monad` trait has made a 'stack-neutral' implementation
    /// of `Monad.Recur` 
    /// </remarks>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Continue value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    [Pure]
    public static K<M, B> recur<M, A, B>(A value, Func<A, K<M, Next<A, B>>> f) 
        where M : Monad<M> =>
        M.Recur(value, f);

    /// <summary>
    /// This is a default implementation of `Monad.Recur` that doesn't use the trampoline.
    /// It's here to use as a placeholder implementation for the trampoline version and for types
    /// where it's unlikely that recursion will be a problem.  NOTE: Using this isn't declarative,
    /// and the users of `Monad.recur` would rightly be miffed if an implementation yielded a
    /// stack-overflow, so use this function with caution.
    /// </summary>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Continue value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    /// <exception cref="BottomException"></exception>
    [Pure]
    public static K<M, B> unsafeRecur<M, A, B>(A value, Func<A, K<M, Next<A, B>>> f)
        where M : Monad<M> =>
        f(value).Bind(n => n switch
                           {
                               { IsCont: true, ContValue: var v } => unsafeRecur(v, f),
                               { IsDone: true, DoneValue: var v } => M.Pure(v),
                               _                                  => throw new BottomException()
                           });
    
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
}
