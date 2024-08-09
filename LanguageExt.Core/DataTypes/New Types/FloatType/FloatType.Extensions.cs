using LanguageExt.Traits;
using System.Collections.Generic;

namespace LanguageExt;

public static class FloatTypeExtensions
{
    public static SELF Sum<SELF, FLOATING, A>(this IEnumerable<FloatType<SELF, FLOATING, A>> self)
        where FLOATING : Floating<A>
        where SELF : FloatType<SELF, FLOATING, A> =>
        self.AsIterable().Fold(FloatType<SELF, FLOATING, A>.FromInteger(0), (s, x) => s + x);

    public static SELF Sum<SELF, FLOATING, A, PRED>(this IEnumerable<FloatType<SELF, FLOATING, A, PRED>> self)
        where FLOATING : Floating<A>
        where PRED : Pred<A>
        where SELF : FloatType<SELF, FLOATING, A, PRED> =>
        self.AsIterable().Fold(FloatType<SELF, FLOATING, A, PRED>.FromInteger(0), (s, x) => s + x);
}
