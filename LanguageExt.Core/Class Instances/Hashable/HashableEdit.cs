using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Hash for `Patch` `Edit`
/// </summary>
public struct HashableEdit<EqA, A> : Hashable<Edit<EqA, A>> where EqA : Eq<A>
{
    [Pure]
    public static int GetHashCode(Edit<EqA, A> x) => 
        x.GetHashCode();
}
