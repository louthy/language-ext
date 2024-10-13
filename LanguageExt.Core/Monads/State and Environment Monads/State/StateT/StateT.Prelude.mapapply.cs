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
    public static StateT<S, M, B> map<S, M, A, B>(Func<A, B> f, K<StateT<S, M>, A> ma)
        where M : Monad<M>, SemiAlternative<M> =>
        Functor.map(f, ma).As();

    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static StateT<S, M, B> action<S, M, A, B>(K<StateT<S, M>, A> ma, K<StateT<S, M>, B> mb)
        where M : Monad<M>, SemiAlternative<M> =>
        Applicative.action(ma, mb).As();

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
    public static StateT<S, M, B> apply<S, M, A, B>(K<StateT<S, M>, Func<A, B>> mf, K<StateT<S, M>, A> ma) 
        where M : Monad<M>, SemiAlternative<M> =>
        Applicative.apply(mf, ma).As();
}    
