using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
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
        public static MA MPlus<MPLUS, MA, A>(this MA x, MA y) 
            where MPLUS : struct, MonadPlus<MA, A> =>
            mplus<MPLUS, MA, A>(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        public static MA MSum<MPLUS, MA, A>(this IEnumerable<MA> xs) where MPLUS : struct, MonadPlus<MA, A> =>
            msum<MPLUS, MA, A>(xs);

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) monad</returns>
        public static MA Filter<MPLUS, MA, A>(this MA ma, Func<A, bool> pred) where MPLUS : struct, MonadPlus<MA, A> =>
            filter<MPLUS, MA, A>(ma, pred);
    }
}
