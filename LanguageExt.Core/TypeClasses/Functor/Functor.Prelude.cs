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
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="FunctorAB">Functor instance from A to B</typeparam>
        /// <typeparam name="FA">Source functor type</typeparam>
        /// <typeparam name="FB">Target functor type</typeparam>
        /// <typeparam name="A">Source functor bound value type</typeparam>
        /// <typeparam name="B">Target functor bound value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FB map<FunctorAB, FA, FB, A, B>(FA ma, Func<A, B> f)
            where FunctorAB : Functor<FA, FB, A, B> =>
            default(FunctorAB).Map(ma, f);
    }
}
