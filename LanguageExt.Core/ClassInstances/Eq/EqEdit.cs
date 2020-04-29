using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality instance for `Patch` `Edit`
    /// </summary>
    public struct EqEdit<EqA, A> : Eq<Edit<EqA, A>> where EqA : struct, Eq<A>
    {
        public bool Equals(Edit<EqA, A> x, Edit<EqA, A> y) => 
            x == y;

        public int GetHashCode(Edit<EqA, A> x) =>
            default(HashableEdit<EqA, A>).GetHashCode(x);
    }
}
