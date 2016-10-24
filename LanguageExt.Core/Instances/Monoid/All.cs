using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Booleans form a monoid under disjunctions.
    /// </summary>
    public struct All : Monoid<bool>
    {
        public static readonly All Inst = default(All);

        public bool Append(bool x, bool y) => x && y;
        public bool Empty() => true;
    }
}
