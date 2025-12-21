using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IterableNEExtensions
{
    /// <param name="f">Mapping function</param>
    extension<A, B>(Func<A, B> f)
    {
        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the functor, passes it to the map function `f` provided, and
        /// then takes the mapped value and wraps it back up into a new functor.
        /// </remarks>
        /// <param name="ma">Functor to map</param>
        /// <returns>Mapped functor</returns>
        public IterableNE<B> Map(K<IterableNE, A> ma) =>
            Functor.map(f, ma).As();

        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the functor, passes it to the map function `f` provided, and
        /// then takes the mapped value and wraps it back up into a new functor.
        /// </remarks>
        /// <param name="ma">Functor to map</param>
        /// <returns>Mapped functor</returns>
        public IterableNE<B> Map(IterableNE<A> ma) =>
            Functor.map(f, ma).As();
    }

    extension<A>(K<IterableNE, A> ma)
    {
        /// <summary>
        /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
        /// </summary>
        public IterableNE<B> Action<B>(K<IterableNE, B> mb) =>
            Applicative.action(ma, mb).As();
    }    
    
    extension<A>(IterableNE<A> ma)
    {
        /// <summary>
        /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
        /// </summary>
        public IterableNE<B> Action<B>(K<IterableNE, B> mb) =>
            Applicative.action(ma, mb).As();
    }

    /// <param name="mf">Mapping function(s)</param>
    extension<A, B>(IterableNE<Func<A, B>> mf)
    {
        /// <summary>
        /// Applicative functor apply operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
        /// then takes the resulting value and wraps it back up into a new applicative-functor.
        /// </remarks>
        /// <param name="ma">Value(s) applicative functor</param>
        /// <returns>Mapped applicative functor</returns>
        public IterableNE<B> Apply(K<IterableNE, A> ma) =>
            Applicative.apply(mf, ma).As();
    }

    /// <param name="mf">Mapping function(s)</param>
    extension<A, B>(K<IterableNE, Func<A, B>> mf)
    {
        /// <summary>
        /// Applicative functor apply operation
        /// </summary>
        /// <remarks>
        /// Unwraps the value within the `ma` applicative-functor, passes it to the unwrapped function(s) within `mf`, and
        /// then takes the resulting value and wraps it back up into a new applicative-functor.
        /// </remarks>
        /// <param name="ma">Value(s) applicative functor</param>
        /// <returns>Mapped applicative functor</returns>
        public IterableNE<B> Apply(K<IterableNE, A> ma) =>
            Applicative.apply(mf, ma).As();
    }    
}    
