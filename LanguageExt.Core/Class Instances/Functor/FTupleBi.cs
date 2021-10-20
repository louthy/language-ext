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
    public struct FTupleBi<A, B, U, V> :
        BiFunctor<Tuple<A, B>, Tuple<U, V>, A, B, U, V>,
        BiFunctor<ValueTuple<A, B>, ValueTuple<U, V>, A, B, U, V>
    {
        public static readonly FTupleBi<A, B, U, V> Inst = default(FTupleBi<A, B, U, V>);

        /// <summary>
        /// Maps both items in the tuple
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="fa">First item mapping operation</param>
        /// <param name="fb">Second item mapping operation</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public ValueTuple<U, V> BiMap(ValueTuple<A, B> ma, Func<A, U> fa, Func<B, V> fb) =>
            VTuple(fa(ma.Item1), fb(ma.Item2));

        /// <summary>
        /// Maps both items in the tuple
        /// </summary>
        /// <param name="ma">Source tuple</param>
        /// <param name="fa">First item mapping operation</param>
        /// <param name="fb">Second item mapping operation</param>
        /// <returns>Target tuple</returns>
        [Pure]
        public Tuple<U, V> BiMap(Tuple<A, B> ma, Func<A, U> fa, Func<B, V> fb) =>
            Tuple(fa(ma.Item1), fb(ma.Item2));
    }
}
