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
        ///     None   'plus' Some b = None
        ///     Some a 'plus' Some b = Some a
        /// 
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>x 'plus' y </returns>
        public static MA MPlus<MA, A>(this MA x, MA y) where MA : struct, MonadPlus<A> =>
            mplus<MA, A>(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        public static MA MSum<MA, A>(params MA[] xs) where MA : struct, MonadPlus<A> =>
            msum<MA, A>(xs);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        public static MA MSum<MA, A>(IEnumerable<MA> xs) where MA : struct, MonadPlus<A> =>
            msum<MA, A>(xs);

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) monad</returns>
        public static MA Filter<MA, A>(MA ma, Func<A, bool> pred) where MA : struct, MonadPlus<A> =>
            filter(ma, pred);
    }
}
