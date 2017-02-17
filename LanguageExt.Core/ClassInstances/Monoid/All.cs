using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Booleans form a monoid under disjunctions.
    /// </summary>
    public struct All : Monoid<bool>
    {
        public static readonly All Inst = default(All);

        [Pure]
        public bool Append(bool x, bool y) => x && y;

        [Pure]
        public bool Empty() => true;
    }
}
