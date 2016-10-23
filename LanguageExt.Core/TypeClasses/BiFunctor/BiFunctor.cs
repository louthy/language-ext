using System;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass]
    public interface BiFunctor<FA, FC, A, B, C>
    {
        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        FC BiMap(FA ma, Func<A, C> fa, Func<B, C> fb);
    }

    /// <summary>
    /// Functor type-class
    /// </summary>
    [Typeclass]
    public interface BiFunctor<FA, FD, A, B, C, D>
    {
        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="ma">Functor value to map from </param>
        /// <param name="fa">Projection function</param>
        /// <param name="fb">Projection function</param>
        /// <returns>Mapped functor</returns>
        FD BiMap(FA ma, Func<A, C> fa, Func<B, D> fb);
    }

}
