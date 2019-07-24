using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Maps the first item in a tuple 2
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleFst<A, B, R> :
        Functor<Tuple<A, B>, Tuple<R, B>, A, R>,
        Functor<ValueTuple<A, B>, ValueTuple<R, B>, A, R>
    {
        public static readonly FTupleFst<A, B, R> Inst = default(FTupleFst<A, B, R>);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<R, B> Map(Tuple<A, B> ma, Func<A, R> f) =>
            Tuple(f(ma.Item1), ma.Item2);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<R, B> Map(ValueTuple<A, B> ma, Func<A, R> f) =>
            VTuple(f(ma.Item1), ma.Item2);
    }

    /// <summary>
    /// Maps the first item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleFst<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<R, B, C>, A, R>,
        Functor<ValueTuple<A, B, C>, ValueTuple<R, B, C>, A, R>
    {
        public static readonly FTupleFst<A, B, C, R> Inst = default(FTupleFst<A, B, C, R>);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<R, B, C> Map(Tuple<A, B, C> ma, Func<A, R> f) =>
            Tuple(f(ma.Item1), ma.Item2, ma.Item3);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<R, B, C> Map(ValueTuple<A, B, C> ma, Func<A, R> f) =>
            VTuple(f(ma.Item1), ma.Item2, ma.Item3);
    }
}
