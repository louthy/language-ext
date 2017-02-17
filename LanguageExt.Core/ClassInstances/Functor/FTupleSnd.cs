using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Maps the second item in a tuple 2
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleSnd<A, B, R> :
        Functor<Tuple<A, B>, Tuple<A, R>, B, R>,
        Functor<ValueTuple<A, B>, ValueTuple<A, R>, B, R>
    {
        public static readonly FTupleSnd<A, B, R> Inst = default(FTupleSnd<A, B, R>);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<A, R> Map(ValueTuple<A, B> ma, Func<B, R> f) =>
            VTuple(ma.Item1, f(ma.Item2));

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<A, R> Map(Tuple<A, B> ma, Func<B, R> f) =>
            Tuple(ma.Item1, f(ma.Item2));
    }

    /// <summary>
    /// Maps the second item in a tuple 3
    /// </summary>
    /// <typeparam name="A">First item type</typeparam>
    /// <typeparam name="B">Second item type</typeparam>
    /// <typeparam name="C">Third item type</typeparam>
    /// <typeparam name="R">Target type</typeparam>
    public struct FTupleSnd<A, B, C, R> :
        Functor<Tuple<A, B, C>, Tuple<A, R, C>, B, R>,
        Functor<ValueTuple<A, B, C>, ValueTuple<A, R, C>, B, R>
    {
        public static readonly FTupleSnd<A, B, C, R> Inst = default(FTupleSnd<A, B, C, R>);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<A, R, C> Map(ValueTuple<A, B, C> ma, Func<B, R> f) =>
            VTuple(ma.Item1, f(ma.Item2), ma.Item3);

        /// <summary>
        /// Maps the third item in a tuple 3
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="f">Mapping function</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<A, R, C> Map(Tuple<A, B, C> ma, Func<B, R> f) =>
            Tuple(ma.Item1, f(ma.Item2), ma.Item3);
    }
}
