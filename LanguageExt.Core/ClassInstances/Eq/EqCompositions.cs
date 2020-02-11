using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct EqCompositions<A> : Eq<Compositions<A>>
    {
        public bool Equals(Compositions<A> x, Compositions<A> y) =>
            x == y;

        public int GetHashCode(Compositions<A> x) =>
            default(HashableCompositions<A>).GetHashCode(x);
    }
}
