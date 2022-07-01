using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass("BiF*")]
    public interface BiFunctor<in FAB, out FUV, out A, out B, in U, in V> : Typeclass
    {
        /// <summary>
        /// Projection from one value to another.  Both elements of
        /// the bi-functor can will be mapped to a new result value.
        /// </summary>
        /// <typeparam name="FAB">Source functor value type</typeparam>
        /// <typeparam name="FUV">Target functor value type</typeparam>
        /// <typeparam name="A">Source item 1 value type</typeparam>
        /// <typeparam name="B">Source item 2 value type</typeparam>
        /// <typeparam name="U">Target item 1 value type</typeparam>
        /// <typeparam name="V">Target item 2 value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FUV BiMap(FAB ma, Func<A, U> fa, Func<B, V> fb);
    }
}
