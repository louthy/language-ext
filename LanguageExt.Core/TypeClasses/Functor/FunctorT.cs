using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass]
    public interface FunctorT<MA, A> where MA : Functor<A>
    {
        /// <summary>
        /// Projection from one value to another using f
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="fa">Functor value to map from </param>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        FunctorT<MB, B> Map<MB, B>(FunctorT<MA, A> fa, Func<A, B> f) where MB : struct, Monad<B>;
    }
}
