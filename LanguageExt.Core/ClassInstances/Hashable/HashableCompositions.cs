using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct HashableCompositions<A> : Hashable<Compositions<A>>
    {
        public int GetHashCode(Compositions<A> x) =>
            x.GetHashCode();
    }
}
