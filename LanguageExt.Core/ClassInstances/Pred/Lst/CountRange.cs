using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct CountRange<A, MIN, MAX> : Pred<IReadOnlyList<A>>
        where MIN : struct, Const<int>
        where MAX : struct, Const<int>
    {
        public static readonly CountRange<A, MIN, MAX> Is = default(CountRange<A, MIN, MAX>);

        [Pure]
        public bool True(IReadOnlyList<A> value) =>
            value.Count >= default(MIN).Value && value.Count <= default(MAX).Value;
    }
}
