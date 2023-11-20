#nullable enable
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt
{
    public static partial class Prelude
    {
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
}
