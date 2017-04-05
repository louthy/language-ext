using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.ClassInstances.Pred
{
    /// <summary>
    /// Lst must be non-empty
    /// </summary>
    public struct NonEmpty<A> : Pred<IReadOnlyList<A>>
    {
        public bool True(IReadOnlyList<A> value) =>
            (value?.Count ?? 0) != 0;
    }
}
