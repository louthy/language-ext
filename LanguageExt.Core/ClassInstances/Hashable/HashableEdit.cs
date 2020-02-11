using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash for `Patch` `Edit`
    /// </summary>
    public struct HashableEdit<EqA, A> : Hashable<Edit<EqA, A>> where EqA : struct, Eq<A>
    {
        public int GetHashCode(Edit<EqA, A> x) => x?.GetHashCode() ?? 0;
    }
}
