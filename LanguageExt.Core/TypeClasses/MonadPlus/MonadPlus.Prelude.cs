using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Return monad 'zero'.  None for Option, [] for Lst, ...
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Zero for the structure</returns>
        [Pure]
        public static MA mzero<MPLUS, MA, A>() where MPLUS : struct, MonadPlus<MA, A> =>
            default(MPLUS).Zero();

        /// <summary>
        /// Return monad x 'plus' monad y
        /// 
        /// Note, this doesn't add the bound values, it works on the monad state
        /// itself.  
        /// 
        /// For example with Option:
        /// 
        ///     None   'plus' None   = None
        ///     Some a 'plus' None   = Some a
        ///     None   'plus' Some b = Some b
        ///     Some a 'plus' Some b = Some a
        /// 
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>x 'plus' y </returns>
        [Pure]
        public static MA mplus<MPLUS, MA, A>(MA x, MA y) where MPLUS : struct, MonadPlus<MA, A> =>
            default(MPLUS).Plus(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msum<MPLUS, MA, A>(params MA[] xs) where MPLUS : struct, MonadPlus<MA, A> =>
            xs.Fold(mzero<MPLUS, MA, A>(), (s, x) => mplus<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        [Pure]
        public static MA msum<MPLUS, MA, A>(IEnumerable<MA> xs) where MPLUS : struct, MonadPlus<MA, A> =>
            xs.Fold(mzero<MPLUS, MA, A>(), (s, x) => mplus<MPLUS, MA, A>(s, x));

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) monad</returns>
        [Pure]
        public static MA filter<MPLUS, MA, A>(MA ma, Func<A, bool> pred) where MPLUS : struct, MonadPlus<MA, A> =>
            default(MPLUS).Bind<MPLUS, MA, A>(ma, 
                x => pred(x)
                    ? ma
                    : mzero<MPLUS, MA, A>());
    }
}
