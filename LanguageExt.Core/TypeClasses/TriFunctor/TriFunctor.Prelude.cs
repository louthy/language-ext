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
        /// Projection from one tri-functor to another.  This operation
        /// should map only one of the items (A, B, or C).  The type R
        /// should match A, B, or C depending on which item is being 
        /// mapped.
        /// </summary>
        /// <typeparam name="FABC">Source functor value type</typeparam>
        /// <typeparam name="FR">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="C">Source item 3 value type</typeparam>
        /// <typeparam name="R">Target item value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FR trimap<TriFunctorABC, FABC, FR, A, B, C, R>(FABC ma, Func<A, R> fa, Func<B, R> fb, Func<B, R> fc)
            where TriFunctorABC : TriFunctor<FABC, FR, A, B, C, R> =>
            default(TriFunctorABC).TriMap(ma, fa, fb, fc);

        /// <summary>
        /// Projection from one value to another.  All three elements of
        /// the tri-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FABC">Source functor value type</typeparam>
        /// <typeparam name="FTUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="C">Source item 3 value type</typeparam>
        /// <typeparam name="T">Target item 1 value type</typeparam>
        /// <typeparam name="U">Target item 2 value type</typeparam>
        /// <typeparam name="V">Target item 3 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <param name="fc">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FTUV trimap<TriFunctorABC, FABC, FTUV, A, B, C, T, U, V>(FABC ma, Func<A, T> fa, Func<B, U> fb, Func<C, V> fc)
            where TriFunctorABC : TriFunctor<FABC, FTUV, A, B, C, T, U, V> =>
            default(TriFunctorABC).TriMap(ma, fa, fb, fc);
    }
}
