using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append an extra item to the tuple
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, T3> append<T1, T2, T3>(ValueTuple<T1, T2> self, T3 third) =>
            self.Append(third);

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static int sum<T1, T2>(ValueTuple<int, int> self) =>
            self.Sum();

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static double sum<T1, T2>(ValueTuple<double, double> self) =>
            self.Sum();

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static float sum<T1, T2>(ValueTuple<float, float> self) =>
            self.Sum();

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static decimal sum<T1, T2>(ValueTuple<decimal, decimal> self) =>
            self.Sum();

        /// <summary>
        /// Map to R
        /// </summary>
        [Pure]
        public static R map<T1, T2, R>(ValueTuple<T1, T2> self, Func<T1, T2, R> map) =>
            self.Map(map);

        /// <summary>
        /// Map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<R1, R2> map<T1, T2, R1, R2>(ValueTuple<T1, T2> self, Func<ValueTuple<T1, T2>, ValueTuple<R1, R2>> map) =>
            self.Map(map);

        /// <summary>
        /// Bi-map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<R1, R2> bimap<T1, T2, R1, R2>(ValueTuple<T1, T2> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap) =>
            self.BiMap(firstMap, secondMap);

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2>(ValueTuple<T1, T2> self, Action<T1> first, Action<T2> second) =>
            self.Iter(first, second);

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2>(ValueTuple<T1, T2> self, ValueTuple<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<T1, T2, S>(ValueTuple<T1, T2> self, S state, Func<S, T1, T2, S> fold) =>
            self.Fold(state, fold);

        /// <summary>
        /// Bi-fold
        /// </summary>
        [Pure]
        public static S bifold<T1, T2, S>(ValueTuple<T1, T2> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold) =>
            self.BiFold(state, firstFold, secondFold);

        /// <summary>
        /// Bi-fold back
        /// </summary>
        [Pure]
        public static S bifoldBack<T1, T2, S>(ValueTuple<T1, T2> self, S state, Func<S, T2, S> firstFold, Func<S, T1, S> secondFold) =>
            self.BiFoldBack(state, firstFold, secondFold);
    }
}