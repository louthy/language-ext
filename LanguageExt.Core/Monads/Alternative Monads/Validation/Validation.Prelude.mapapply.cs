using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static Validation<F, B> map<F, A, B>(Func<A, B> f, K<Validation<F>, A> ma) 
        where F : Monoid<F> =>
        ma.As().Map(f);
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Validation<F, B> action<F, A, B>(K<Validation<F>, A> ma, K<Validation<F>, B> mb) 
        where F : Monoid<F> =>
        ma.As().Action(mb);    

    /// <summary>
    /// Applicative functor apply operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
    /// then takes the resulting value and wraps it back up into a new applicative-functor.
    /// </remarks>
    /// <param name="ma">Value(s) applicative functor</param>
    /// <param name="mf">Mapping function(s)</param>
    /// <returns>Mapped applicative functor</returns>
    public static Validation<F, B> apply<F, A, B>(K<Validation<F>, Func<A, B>> mf, K<Validation<F>, A> ma)
        where F : Monoid<F> =>
        mf.As().Apply(ma);
}    
