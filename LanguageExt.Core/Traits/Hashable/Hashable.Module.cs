using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static class Hashable
{
    /// <summary>
    /// Get the hash-code of the provided value
    /// </summary>
    /// <returns>Hash code of x</returns>
    [Pure]
    public static int code<A>(A x) where A : Hashable<A> =>
        A.GetHashCode(x);
}
