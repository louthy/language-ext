using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Semigroup
{
    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static A combine<A>(A x, A y) where A : Semigroup<A> =>
        x + y;
    
    /// <summary>
    /// Get a concrete semigroup instance value from a semigroup supporting trait-type
    /// </summary>
    /// <typeparam name="A">Semigroup type</typeparam>
    /// <returns>Semigroup instance that can be passed around as a value</returns>
    [Pure]
    public static SemigroupInstance<A> instance<A>()
        where A : Semigroup<A> =>
        A.Instance;    
}
