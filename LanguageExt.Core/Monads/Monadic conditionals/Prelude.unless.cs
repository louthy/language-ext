using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Conditional execution of `Applicative` expressions
    /// 
    /// Run the `alternative` when the `flag` is `false`, return `pure ()` when `true`
    /// </summary>
    /// <param name="flag">If `false` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the flag is `false`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `false` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in unless(x == 100, Console.writeLine〈RT〉("x should be 100!"))
    ///     select x;
    /// 
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<F, Unit> unless<F>(bool flag, K<F, Unit> alternative)
        where F : Applicative<F> =>
        Applicative.unless(flag, alternative);
    
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
    public static K<M, Unit> unless<M>(K<M, bool> Pred, K<IO, Unit> Then)
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
    
}
