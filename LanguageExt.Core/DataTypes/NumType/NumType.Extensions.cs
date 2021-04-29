using LanguageExt.TypeClasses;
using System.Collections.Generic;

namespace LanguageExt
{
    public static class NumTypeExtensions
    {
        public static SELF Sum<SELF, NUM, A>(this IEnumerable<NumType<SELF, NUM, A>> self)
            where SELF : NumType<SELF, NUM, A>
            where NUM : struct, Num<A> =>
            (SELF) self.Reduce((s, x) => s + x);

        public static SELF Sum<SELF, NUM, A, PRED>(this IEnumerable<NumType<SELF, NUM, A, PRED>> self)
            where SELF : NumType<SELF, NUM, A, PRED>
            where NUM : struct, Num<A>
            where PRED : struct, Pred<A> =>
            (SELF) self.Reduce((s, x) => s + x);
    }
}
