using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct Range<ORD, A, MIN, MAX> : Pred<A>
        where ORD : struct, Ord<A>
        where MIN : struct, Const<A>
        where MAX : struct, Const<A>
    {
        public static readonly Range<ORD, A, MIN, MAX> Is = default(Range<ORD, A, MIN, MAX>);

        [Pure]
        public bool True(A value) =>
            GreaterOrEq<ORD, A, MIN>.Is.True(value) && LessOrEq<ORD, A, MAX>.Is.True(value);
    }
}
