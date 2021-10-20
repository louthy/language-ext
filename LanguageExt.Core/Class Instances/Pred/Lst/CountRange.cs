using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct CountRange<MIN, MAX> : Pred<ListInfo>
        where MIN : struct, Const<int>
        where MAX : struct, Const<int>
    {
        public static readonly CountRange<MIN, MAX> Is = default(CountRange<MIN, MAX>);

        [Pure]
        public bool True(ListInfo value) =>
            value.Count >= default(MIN).Value && value.Count <= default(MAX).Value;
    }
}
