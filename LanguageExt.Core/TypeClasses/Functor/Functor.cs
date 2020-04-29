using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    /// <typeparam name="FA">Source functor type</typeparam>
    /// <typeparam name="FB">Target functor type</typeparam>
    /// <typeparam name="A">Source functor bound value type</typeparam>
    /// <typeparam name="B">Target functor bound value type</typeparam>
    [Typeclass("F*")]
    public interface Functor<FA, FB, A, B> : Typeclass
    {
        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FB Map(FA ma, Func<A, B> f);
    }
}
