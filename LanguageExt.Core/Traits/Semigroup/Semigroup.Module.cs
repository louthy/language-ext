using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class Semigroup
{
    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static A append<A>(A x, A y) where A : Semigroup<A> =>
        x + y;
}
