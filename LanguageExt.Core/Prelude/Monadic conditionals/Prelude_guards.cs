using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using LanguageExt.ClassInstances;
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
        public static Guard<E> guard<E>(bool flag, Func<E> False) =>
            new Guard<E>(flag, False);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="False">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<E> guard<E>(bool flag, E False) =>
            new Guard<E>(flag, False);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="False">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<Error> guard(bool flag, Func<Error> False) =>
            new Guard<Error>(flag, False);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="False">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<Error> guard(bool flag, Error False) =>
            new Guard<Error>(flag, False);
        
        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="True">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<E> guardnot<E>(bool flag, Func<E> True) =>
            new Guard<E>(!flag, True);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="True">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<E> guardnot<E>(bool flag, E True) =>
            new Guard<E>(!flag, True);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="True">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<Error> guardnot(bool flag, Func<Error> True) =>
            new Guard<Error>(!flag, True);

        /// <summary>
        /// Guard against continuing a monadic expression
        /// </summary>
        /// <param name="flag">Flag for continuing</param>
        /// <param name="True">If the flag is false, this provides the error</param>
        /// <returns>Guard</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guard<Error> guardnot(bool flag, Error True) =>
            new Guard<Error>(!flag, True);
    }
}
