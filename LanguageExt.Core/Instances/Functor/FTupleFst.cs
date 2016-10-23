using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Maps the first item in a tuple 2
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleFst<A, B, R> :
        Functor<Tuple<A, B>, Tuple<R, B>, A, R>
    {
        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        public Tuple<R, B> Map(Tuple<A, B> ma, Func<A, R> f) =>
            new Tuple<R, B>(f(ma.Item1), ma.Item2);
    }

    /// <summary>
    /// Maps the first item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleFst<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<R, B, C>, A, R>
    {
        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        public Tuple<R, B, C> Map(Tuple<A, B, C> ma, Func<A, R> f) =>
            new Tuple<R, B, C>(f(ma.Item1), ma.Item2, ma.Item3);
    }
}
