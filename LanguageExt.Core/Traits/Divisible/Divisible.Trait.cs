using System;

namespace LanguageExt.Traits;

/// <summary>
/// A `Divisible` contravariant functor is the contravariant analogue of `Applicative`.
///
/// Continuing the intuition that 'Contravariant' functors (`Cofunctor`) consume input, a 'Divisible'
/// contravariant functor also has the ability to be composed "beside" another contravariant
/// functor.
/// </summary>
/// <typeparam name="F">Self referring type</typeparam>
public interface Divisible<F> : Cofunctor<F>
{
    /// <summary>
    /// If one can handle split `a` into `(b, c)`, as well as handle `b`s and `c`s, then one can handle `a`s
    /// </summary>
    public static abstract K<F, A> Divide<A, B, C>(Func<A, (B Left, C Right)> f, K<F, B> fb, K<F, C> fc);
    
    /// <summary>
    /// Conquer acts as an identity for combining `Divisible` functors.
    /// </summary>
    public static abstract K<F, A> Conquer<A>();
}
