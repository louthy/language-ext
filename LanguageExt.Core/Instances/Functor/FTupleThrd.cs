using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Maps the third item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam> 
    public struct FTupleThrd<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<A, B, R>, C, R>
    {
        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        public Tuple<A, B, R> Map(Tuple<A, B, C> ma, Func<C, R> f) =>
            new Tuple<A, B, R>(ma.Item1, ma.Item2, f(ma.Item3));
    }
}
