using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Applicative type-class
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    [Typeclass("Appl*")]
    public interface ApplicativePure<FA, A> : Typeclass
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
    /// <typeparam name="A">Bound value</typeparam>
    /// <typeparam name="B">Type of the bound return value</typeparam>
    [Typeclass("Appl*")]
    public interface Applicative<FAB, FA, FB, A, B> : ApplicativePure<FA, A>
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

    /// <summary>
    /// Applicative type-class
    /// </summary>
    /// <typeparam name="FABC">Type of the applicative computation: f(a -> b -> c)</typeparam>
    /// <typeparam name="FBC">Type of the applicative computation to return: f(b -> c)</typeparam>
    /// <typeparam name="FA">Type of the applicative of the first argument to apply</typeparam>
    /// <typeparam name="FB">Type of the applicative of the second argument to apply</typeparam>
    /// <typeparam name="FC">Type of the applicative to return</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <typeparam name="B">Type of the bound return value</typeparam>
    [Typeclass("Appl*")]
    public interface Applicative<FABC, FBC, FA, FB, FC, A, B, C> : ApplicativePure<FA, A>
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative to apply</param>
        /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
        [Pure]
        FBC Apply(FABC fabc, FA fa);

        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="fab">Function to apply the applicative to</param>
        /// <param name="fa">Applicative a to apply</param>
        /// <param name="fb">Applicative b to apply</param>
        /// <returns>Applicative of type FC derived from Applicative of C</returns>
        [Pure]
        FC Apply(FABC fabc, FA fa, FB fb);
    }
}
