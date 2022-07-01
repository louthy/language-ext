using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Applicative type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass("Appl*")]
    public interface ApplicativePure<out FA, in A> : Typeclass
    {
        /// <summary>
        /// Applicative return
        /// </summary>
        /// <typeparam name="A">Type of the bound applicative value</typeparam>
        /// <param name="x">The bound applicative value</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        FA Pure(A x);
    }

    /// <summary>
    /// Applicative type-class
    /// </summary>
    /// <typeparam name="FAB">Type of the applicative computation: f(a -> b)</typeparam>
    /// <typeparam name="FA">Type of the applicative to apply</typeparam>
    /// <typeparam name="FB">Type of the applicative to return</typeparam>
    /// <typeparam name="A">Bound input value</typeparam>
    /// <typeparam name="B">Bound output value</typeparam>
    [Typeclass("Appl*")]
    public interface Applicative<in FAB, FA, FB, A, B> :
        Functor<FA, FB, A, B>,
        ApplicativePure<FA, A>
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        FB Apply(FAB fab, FA fa);

        /// <summary>
        /// Evaluate fa, then fb, ignoring the result of fa
        /// </summary>
        /// <param name="fa">Applicative to evaluate first</param>
        /// <param name="fb">Applicative to evaluate second and then return</param>
        /// <returns>Applicative of type FB derived from Applicative of B</returns>
        [Pure]
        FB Action(FA fa, FB fb);
    }
}
