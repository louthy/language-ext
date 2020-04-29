using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOption<A> : Eq<Option<A>>
    {
        public bool Equals(Option<A> x, Option<A> y) =>
            default(EqOptional<MOption<A>, Option<A>, A>).Equals(x, y);

        public int GetHashCode(Option<A> x) =>
            default(HashableOption<A>).GetHashCode(x);
    }
}
