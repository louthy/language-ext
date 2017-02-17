using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Booleans form a monoid under conjunction.
    /// </summary>
    public struct Any : Monoid<bool>
    {
        public static readonly Any Inst = default(Any);

        [Pure]
        public bool Append(bool x, bool y) => x || y;

        [Pure]
        public bool Empty() => false;
    }
}
