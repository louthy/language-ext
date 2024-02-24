using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static partial class SemiAlternative
{
    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static K<F, A> Or<F, A>(this K<F, A> ma, K<F, A> mb)
        where F : SemiAlternative<F> =>
        F.Or(ma, mb);
}
