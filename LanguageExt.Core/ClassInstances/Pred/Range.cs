using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances.Pred
{
    public struct Range<A, ORD, MIN, MAX> : Pred<A>
        where ORD : struct, Ord<A>
        where MIN : struct, Const<A>
        where MAX : struct, Const<A>
    {
        public static readonly Range<A, ORD, MIN, MAX> Is = default(Range<A, ORD, MIN, MAX>);

        public bool True(A value) =>
            GreaterOrEq<A, ORD, MIN>.Is.True(value) && LessOrEq<A, ORD, MAX>.Is.True(value);
    }
}
