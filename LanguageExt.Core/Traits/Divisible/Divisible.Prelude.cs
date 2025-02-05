using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A `Divisible` contravariant functor is the contravariant analogue of `Applicative`.
///
/// Continuing the intuition that 'Contravariant' functors (`Cofunctor`) consume input, a 'Divisible'
/// contravariant functor also has the ability to be composed "beside" another contravariant
/// functor.
/// </summary>
/// <typeparam name="F">Self referring type</typeparam>
public static partial class Prelude
{
    /// <summary>
    /// If one can handle split `a` into `(b, c)`, as well as handle `b`s and `c`s, then one can handle `a`s
    /// </summary>
    public static K<F, A> divide<F, A, B, C>(Func<A, (B Left, C Right)> f, K<F, B> fb, K<F, C> fc) 
        where F : Divisible<F> =>
        F.Divide(f, fb, fc);

    /// <summary>
    /// Conquer acts as an identity for combining `Divisible` functors.
    /// </summary>
    public static K<F, A> conquer<F, A>() 
        where F : Divisible<F> =>
        F.Conquer<A>();
}
