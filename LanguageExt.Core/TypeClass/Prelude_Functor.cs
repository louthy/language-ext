using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public static partial class Prelude
    {
        /// <summary>
        /// Functor map
        /// <summary>
        public static Functor<B> map<FUNCTOR, A, B>(Functor<A> x, Func<A, B> f) where FUNCTOR : struct, Functor<A> =>
            default(FUNCTOR).Map(x, f);
    }
}
