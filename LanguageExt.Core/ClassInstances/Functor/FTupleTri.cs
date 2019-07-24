using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Maps the first item in a tuple 2
    /// </summary>
    /// <typeparam name="A">First item source type</typeparam>
    /// <typeparam name="B">Second item source type</typeparam>
    /// <typeparam name="U">First item source target type</typeparam>
    /// <typeparam name="V">Second item source target type</typeparam>
    public struct FTupleTri<A, B, C, T, U, V> :
        TriFunctor<Tuple<A, B, C>, Tuple<T, U, V>, A, B, C, T, U, V>,
        TriFunctor<ValueTuple<A, B, C>, ValueTuple<T, U, V>, A, B, C, T, U, V>
    {
        public static readonly FTupleTri<A, B, C, T, U, V> Inst = default(FTupleTri<A, B, C, T, U, V>);

        /// <summary>
        /// Maps all three items in the tuple
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="fa">First item mapping operation</param>
        /// <param name="fb">Second item mapping operation</param>
        /// <param name="fc">Third item mapping operation</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<T, U, V> TriMap(ValueTuple<A, B, C> ma, Func<A, T> fa, Func<B, U> fb, Func<C, V> fc) =>
            VTuple(fa(ma.Item1), fb(ma.Item2), fc(ma.Item3));

        /// <summary>
        /// Maps all three items in the tuple
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="fa">First item mapping operation</param>
        /// <param name="fb">Second item mapping operation</param>
        /// <param name="fc">Third item mapping operation</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<T, U, V> TriMap(Tuple<A, B, C> ma, Func<A, T> fa, Func<B, U> fb, Func<C, V> fc) =>
            Tuple(fa(ma.Item1), fb(ma.Item2), fc(ma.Item3));

    }
}
