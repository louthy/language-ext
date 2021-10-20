using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass("F*Async")]
    public interface FunctorAsync<FA, FB, A, B> : Functor<FA, FB, A, B>
    {
        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="FA">Source functor type</typeparam>
        /// <typeparam name="FB">Target functor type</typeparam>
        /// <typeparam name="A">Source functor bound value type</typeparam>
        /// <typeparam name="B">Target functor bound value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        FB MapAsync(FA ma, Func<A, Task<B>> f);
    }
}
