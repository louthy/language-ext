using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Projection from one bi-functor to another.  This operation
        /// should map only one of the items (A, or B).  The type R
        /// should match A, or B depending on which item is being mapped.
        /// </summary>
        /// <typeparam name="BiFunctorAB">struct type derived from BiFunctor</typeparam>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FR bimap<BiFunctorAB, FAB, FR, A, B, R>(FAB ma, Func<A, R> fa, Func<B, R> fb)
            where BiFunctorAB : BiFunctor<FAB, FR, A, B, R> =>
            default(BiFunctorAB).BiMap(ma, fa, fb);

        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FUV bimap<BiFunctorAB, FAB, FUV, A, B, U, V>(FAB ma, Func<A, U> fa, Func<B, V> fb)
            where BiFunctorAB : BiFunctor<FAB, FUV, A, B, U, V> =>
            default(BiFunctorAB).BiMap(ma, fa, fb);
    }
}
