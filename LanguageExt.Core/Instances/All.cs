using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Booleans form a monoid under disjunctions.
    /// </summary>
    public struct Any : Monoid<bool>
    {
        public bool Append(bool x, bool y) => x || y;
        public bool Empty() => true;
    }
}
