using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Maps the third item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam> 
    public struct FTupleThrd<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<A, B, R>, C, R>,
        Functor<ValueTuple<A, B, C>, ValueTuple<A, B, R>, C, R>
    {
        public static readonly FTupleThrd<A, B, C, R> Inst = default(FTupleThrd<A, B, C, R>);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<A, B, R> Map(ValueTuple<A, B, C> ma, Func<C, R> f) =>
            VTuple(ma.Item1, ma.Item2, f(ma.Item3));

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<A, B, R> Map(Tuple<A, B, C> ma, Func<C, R> f) =>
            Tuple(ma.Item1, ma.Item2, f(ma.Item3));
    }
}
