using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

/// <summary>
/// Finds an appropriate Hashable from the loaded assemblies, if one can't be found then it
/// falls back to the standard .NET Object.GetHashCode() method to provide a hash-code.
/// </summary>
public struct HashableDefault<A> : Hashable<A>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    [Pure]
    public static int GetHashCode(A x) =>
        HashableClass<A>.GetHashCode(x);
}
