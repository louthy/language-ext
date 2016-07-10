using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Functor map
        /// <summary>
        public static Functor<B> map<FUNCTOR, A, B>(Functor<A> x, Func<A, B> f) where FUNCTOR : struct, Functor<A> =>
            default(FUNCTOR).Map(x, f);
    }
}
