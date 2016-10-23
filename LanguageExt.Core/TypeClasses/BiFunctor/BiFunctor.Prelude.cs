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
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="fa">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FB bimap<FUNCTORA, FA, FB, A, B>(FA fa, Func<A, B> map)
            where FUNCTORA : Functor<FA, FB, A, B> =>
            default(FUNCTORA).Map(fa, map);
    }
}
