using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type hash
    /// </summary>
    public struct HashableOptionUnsafe<A> : Hashable<OptionUnsafe<A>>
    {
        public int GetHashCode(OptionUnsafe<A> x) =>
            default(HashableOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).GetHashCode(x);
    }
}
