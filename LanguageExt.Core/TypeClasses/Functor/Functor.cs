using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass]
    public interface Functor<FA, FB, A, B>
    {
        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="fa">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        FB Map(FA ma, Func<A, B> f);
    }
}
