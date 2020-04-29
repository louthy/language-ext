using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality instance for `Patch`
    /// </summary>
    /// <typeparam name="EqA"></typeparam>
    /// <typeparam name="A"></typeparam>
    public struct EqPatch<EqA, A> : Eq<Patch<EqA, A>> where EqA : struct, Eq<A>
    {
        public bool Equals(Patch<EqA, A> x, Patch<EqA, A> y) => 
            x == y;

        public int GetHashCode(Patch<EqA, A> x) =>
            default(HashablePatch<EqA, A>).GetHashCode(x);
    }
}
