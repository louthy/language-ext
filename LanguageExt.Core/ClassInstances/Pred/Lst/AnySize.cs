using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    public struct AnySize : Pred<ListInfo>
    {
        public static readonly AnySize Is = default(AnySize);

        [Pure]
        public bool True(ListInfo value) =>
            true;
    }
}
