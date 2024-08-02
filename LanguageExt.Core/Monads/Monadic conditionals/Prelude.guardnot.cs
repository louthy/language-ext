using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
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
