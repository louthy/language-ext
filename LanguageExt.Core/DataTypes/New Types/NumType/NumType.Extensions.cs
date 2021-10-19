using LanguageExt.TypeClasses;
using System.Collections.Generic;

namespace LanguageExt
{
    public static class NumTypeExtensions
    {
        public static SELF Sum<SELF, NUM, A>(this IEnumerable<NumType<SELF, NUM, A>> self)
            where SELF : NumType<SELF, NUM, A>
            where NUM : struct, Num<A> =>
            self.Fold(NumType<SELF, NUM, A>.FromInteger(0), (s, x) => s + x);

        public static SELF Sum<SELF, NUM, A, PRED>(this IEnumerable<NumType<SELF, NUM, A, PRED>> self)
            where SELF : NumType<SELF, NUM, A, PRED>
            where NUM : struct, Num<A>
            where PRED : struct, Pred<A> =>
            self.Fold(NumType<SELF, NUM, A, PRED>.FromInteger(0), (s, x) => s + x);
    }
}
