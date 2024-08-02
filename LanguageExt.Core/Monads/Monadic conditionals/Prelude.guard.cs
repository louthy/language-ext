using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
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
}
