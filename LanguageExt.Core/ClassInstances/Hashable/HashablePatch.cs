using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash for `Patch`
    /// </summary>
    public struct HashablePatch<EqA, A> : Hashable<Patch<EqA, A>> where EqA : struct, Eq<A>
    {
        public int GetHashCode(Patch<EqA, A> x) => x.GetHashCode();
    }
}
