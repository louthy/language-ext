using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct MaxCount<A, MAX> : Pred<IReadOnlyList<A>>
        where MAX : struct, Const<int>
    {
        public static readonly MaxCount<A, MAX> Is = default(MaxCount<A, MAX>);

        [Pure]
        public bool True(IReadOnlyList<A> value) =>
            value.Count <= default(MAX).Value;
    }
}
