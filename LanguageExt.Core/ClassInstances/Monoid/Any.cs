using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Booleans form a monoid under conjunction.
    /// </summary>
    public struct Any : Monoid<bool>
    {
        public static readonly Any Inst = default(Any);

        public bool Append(bool x, bool y) => x || y;
        public bool Empty() => false;
    }
}
