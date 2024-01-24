using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Equality instance for `Patch` `Edit`
    /// </summary>
    public struct EqEdit<EqA, A> : Eq<Edit<EqA, A>> where EqA : Eq<A>
    {
        [Pure]
        public static bool Equals(Edit<EqA, A> x, Edit<EqA, A> y) => 
            x == y;

        [Pure]
        public static int GetHashCode(Edit<EqA, A> x) =>
            HashableEdit<EqA, A>.GetHashCode(x);
    }
}
