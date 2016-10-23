using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Maps the second item in a tuple 2
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleSnd<A, B, R> :
        Functor<Tuple<A, B>, Tuple<A, R>, B, R>
    {
        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        public Tuple<A, R> Map(Tuple<A, B> ma, Func<B, R> f) =>
            new Tuple<A, R>(ma.Item1, f(ma.Item2));
    }

    /// <summary>
    /// Maps the second item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleSnd<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<A, R, C>, B, R>
    { 
        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        public Tuple<A, R, C> Map(Tuple<A, B, C> ma, Func<B, R> f) =>
            new Tuple<A, R, C>(ma.Item1, f(ma.Item2), ma.Item3);
    }
}
