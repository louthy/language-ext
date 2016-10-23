using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="A">Functor value type</typeparam>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="fa">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static FB Select<FUNCTORA, FA, FB, A, B>(
            this FA fa,
            Func<A, B> map)
            where FUNCTORA : Functor<FA, FB, A, B> =>
            default(FUNCTORA).Map(fa, map);
    }
}
