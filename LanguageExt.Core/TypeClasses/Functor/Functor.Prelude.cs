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
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FC map<FUNCTORA, FA, FC, A, B, C>(FA ma, Func<A, C> fa, Func<B, C> fb)
            where FUNCTORA : BiFunctor<FA, FC, A, B, C> =>
            default(FUNCTORA).BiMap(ma, fa, fb);
    }
}
