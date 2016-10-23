using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Fold a list of monoids of A.
        /// </summary>
        public static A MConcat<MONOID, A>(this IEnumerable<A> xs) where MONOID : struct, Monoid<A> =>
            xs.Fold(mempty<MONOID, A>(), (s, x) => append<MONOID, A>(s, x));
    }
}
