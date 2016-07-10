using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    public interface Functor<A> : Seq<A>
    {
        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="fa">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        Functor<B> Map<B>(Functor<A> fa, Func<A, B> f);
    }
}
