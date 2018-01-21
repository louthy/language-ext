using LanguageExt.TypeClasses;
using System.Collections.Generic;

namespace LanguageExt
{
    public static class FloatTypeExtensions
    {
        public static SELF Sum<SELF, FLOATING, A>(this IEnumerable<FloatType<SELF, FLOATING, A>> self)
            where FLOATING : struct, Floating<A>
            where SELF : FloatType<SELF, FLOATING, A> =>
            self.Fold(FloatType<SELF, FLOATING, A>.FromInteger(0), (s, x) => s + x);

        public static SELF Sum<SELF, FLOATING, A, PRED>(this IEnumerable<FloatType<SELF, FLOATING, A, PRED>> self)
            where FLOATING : struct, Floating<A>
            where PRED : struct, Pred<A>
            where SELF : FloatType<SELF, FLOATING, A, PRED> =>
            self.Fold(FloatType<SELF, FLOATING, A, PRED>.FromInteger(0), (s, x) => s + x);
    }
}
