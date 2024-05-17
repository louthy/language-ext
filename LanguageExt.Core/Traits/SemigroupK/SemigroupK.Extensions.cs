using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class SemigroupK
{
    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="mx">The left hand side of the operation</param>
    /// <param name="my">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static K<M, A> Combine<M, A>(this K<M, A> mx, K<M, A> my)
        where M : SemigroupK<M> =>
        M.Combine(mx, my);
}
