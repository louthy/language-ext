using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static EitherT<L, M, B> Action<L, M, A, B>(this EitherT<L, M, A> ma, K<EitherT<L, M>, B> mb)
        where M : Monad<M> =>
        Applicative.action(ma, mb).As();
    
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static EitherT<L, M, B> Action<L, M, A, B>(this K<EitherT<L, M>, A> ma, K<EitherT<L, M>, B> mb)
        where M : Monad<M> =>
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
    public static EitherT<L, M, B> Apply<L, M, A, B>(this EitherT<L, M, Func<A, B>> mf, K<EitherT<L, M>, A> ma) 
        where M : Monad<M> =>
        Applicative.apply(mf, ma).As();

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
    public static EitherT<L, M, B> Apply<L, M, A, B>(this K<EitherT<L, M>, Func<A, B>> mf, K<EitherT<L, M>, A> ma) 
        where M : Monad<M> =>
        Applicative.apply(mf, ma).As();
}    
