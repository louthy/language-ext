using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Eq
{
    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<A>(A x, A y) where A : Eq<A> =>
        A.Equals(x, y);
}
