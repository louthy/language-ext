using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;

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
        public static MA mzero<MA, A>() where MA : struct, MonadPlus<A> =>
            (MA)default(MA).Zero();

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
        public static MA mplus<MA, A>(MA x, MA y) where MA : struct, MonadPlus<A> =>
            (MA)default(MA).Plus(x, y);

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        public static MA msum<MA, A>(params MA[] xs) where MA : struct, MonadPlus<A> =>
            xs.Fold(mzero<MA, A>(), (s, x) => mplus<MA, A>(s, x));

        /// <summary>
        /// Performs the following fold operation: fold(xs, mzero, mplus)
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="xs">The monads to sum</param>
        /// <returns>The summed monads</returns>
        public static MA msum<MA, A>(IEnumerable<MA> xs) where MA : struct, MonadPlus<A> =>
            xs.Fold(mzero<MA, A>(), (s, x) => mplus<MA, A>(s, x));

        /// <summary>
        /// Filters the monad if the predicate doesn't hold
        /// </summary>
        /// <typeparam name="MA">Monad type</typeparam>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="ma">The monads filter</param>
        /// <returns>The filtered (or not) monad</returns>
        public static MA filter<MA, A>(MA ma, Func<A, bool> pred) where MA : struct, MonadPlus<A> =>
            (MA)ma.Bind(ma, 
                x => pred(x)
                    ? Return<MA, A>(x)
                    : mzero<MA, A>());
    }
}
