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
        /// The identity of append
        /// <summary>
        public static A mempty<MONOID, A>() where MONOID : struct, Monoid<A> =>
            default(MONOID).Empty();

        /// <summary>
        /// Fold a list using the monoid.
        /// </summary>
        public static A mconcat<MONOID, A>(IEnumerable<A> xs) where MONOID : struct, Monoid<A> =>
            xs.Fold(mempty<MONOID, A>(), (s, x) => append<MONOID, A>(s, x));

        /// <summary>
        /// Fold a list using the monoid.
        /// </summary>
        public static A mconcat<MONOID, A>(params A[] xs) where MONOID : struct, Monoid<A> =>
            xs.Fold(mempty<MONOID, A>(), (s, x) => append<MONOID, A>(s, x));
    }
}
