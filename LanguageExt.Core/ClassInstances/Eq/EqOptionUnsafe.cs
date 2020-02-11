using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct EqOptionUnsafe<A> : Eq<OptionUnsafe<A>>
    {
        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(EqOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).Equals(x, y);

        public int GetHashCode(OptionUnsafe<A> x) =>
            default(HashableOptionUnsafe<A>).GetHashCode(x);
    }
}
