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
    /// Run the `alternative` when the `flag` is `true`, return `pure ()` when `false`
    /// </summary>
    /// <param name="flag">If `true` the `alternative` is run</param>
    /// <param name="alternative">Computation to run if the `flag` is `true`</param>
    /// <returns>Either the result of the `alternative` computation if the `flag` is `true` or `Unit`</returns>
    /// <example>
    ///
    ///     from x in ma
    ///     from _ in when(x == 100, Console.writeLine<RT>("x is 100, finally!"))
    ///     select x;
    /// 
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<F, Unit> when<F>(bool flag, K<F, Unit> alternative)
        where F : Applicative<F> =>
        Applicative.when(flag, alternative);
    
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
    ///     from _ in unless(x == 100, Console.writeLine<RT>("x should be 100!"))
    ///     select x;
    /// 
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<F, Unit> unless<F>(bool flag, K<F, Unit> alternative)
        where F : Applicative<F> =>
        Applicative.unless(flag, alternative);

    /// <summary>
    /// Guard against continuing an applicative expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <returns>Applicative that yields `()` if `flag` is `true`; otherwise it yields `Applicative.Empty` - 
    /// shortcutting the expression</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static K<F, Unit> guard<F>(bool flag)
        where F : Alternative<F> =>
        flag ? F.Pure(unit) : F.Empty<Unit>();
    
    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="False">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<E, Unit> guard<E>(bool flag, Func<E> False) =>
        new (flag, False);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="False">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<E, Unit> guard<E>(bool flag, E False) =>
        new (flag, False);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="False">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<Error, Unit> guard(bool flag, Func<Error> False) =>
        new (flag, False);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="False">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<Error, Unit> guard(bool flag, Error False) =>
        new (flag, False);
        
    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="True">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<E, Unit> guardnot<E>(bool flag, Func<E> True) =>
        new (!flag, True);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="True">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<E, Unit> guardnot<E>(bool flag, E True) =>
        new (!flag, True);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="True">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<Error, Unit> guardnot(bool flag, Func<Error> True) =>
        new (!flag, True);

    /// <summary>
    /// Guard against continuing a monadic expression
    /// </summary>
    /// <param name="flag">Flag for continuing</param>
    /// <param name="True">If the flag is false, this provides the error</param>
    /// <returns>Guard</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guard<Error, Unit> guardnot(bool flag, Error True) =>
        new (!flag, True);    
}
