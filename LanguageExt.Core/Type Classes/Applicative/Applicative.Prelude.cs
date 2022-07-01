using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="f">Applicative function</param>
        /// <param name="a">Applicative value</param>
        /// <typeparam name="F">Applicative functor</typeparam>
        /// <typeparam name="FAB">a -> b type</typeparam>
        /// <typeparam name="FA">Applicative a type</typeparam>
        /// <typeparam name="FB">Applicative b type</typeparam>
        /// <typeparam name="A">a type</typeparam>
        /// <typeparam name="B">b type</typeparam>
        /// <returns></returns>
        public static FB apply<F, FAB, FA, FB, A, B>(FAB f, FA a)
            where F : struct, Applicative<FAB, FA, FB, A, B> =>
            default(F).Apply(f, a);
        
    }
}
