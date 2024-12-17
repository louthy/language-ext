using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Conditional execution of `Applicative` expressions
    /// 
    /// Run the `alternative` when the `flag` is `true`, return `pure ()` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Console.writeLine〈RT〉("x is 100, finally!"))
    ///     select x;
    /// 
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<F, Unit> when<F>(bool flag, K<F, Unit> alternative)
        where F : Applicative<F> =>
        Applicative.when(flag, alternative);
    
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
    public static K<M, Unit> when<M>(K<M, bool> Pred, K<IO, Unit> Then)
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
        Pred.Bind(f => Applicative.when(f, M.Pure(unit)));    
}
