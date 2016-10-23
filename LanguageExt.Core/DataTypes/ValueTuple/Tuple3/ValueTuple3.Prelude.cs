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
        public static ValueTuple<T1, T2, T3, T4> append<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3> self, T4 fourth) =>
            ValueTuple.Create(self.Item1, self.Item2, self.Item3, fourth);

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static int sum(ValueTuple<int, int, int> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static double sum(ValueTuple<double, double, double> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static float sum(ValueTuple<float, float, float> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static decimal sum(ValueTuple<decimal, decimal, decimal> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Map to R
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, R>(ValueTuple<T1, T2, T3> self, Func<T1, T2, T3, R> map) =>
            map(self.Item1, self.Item2, self.Item3);

        /// <summary>
        /// Map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<R1, R2, R3> map<T1, T2, T3, R1, R2, R3>(ValueTuple<T1, T2, T3> self, Func<ValueTuple<T1, T2, T3>, ValueTuple<R1, R2, R3>> map) =>
            map(self);

        /// <summary>
        /// Tri-map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<R1, R2, R3> trimap<T1, T2, T3, R1, R2, R3>(ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap, Func<T3, R3> thirdMap) =>
            ValueTuple.Create(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3));

        /// <summary>
        /// First item-map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<R1, T2, T3> mapFirst<T1, T2, T3, R1>(ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap) =>
            ValueTuple.Create(firstMap(self.Item1), self.Item2, self.Item3);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<T1, R2, T3> mapSecond<T1, T2, T3, R2>(ValueTuple<T1, T2, T3> self, Func<T2, R2> secondMap) =>
            ValueTuple.Create(self.Item1, secondMap(self.Item2), self.Item3);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static ValueTuple<T1, T2, R3> mapThird<T1, T2, T3, R3>(ValueTuple<T1, T2, T3> self, Func<T3, R3> thirdMap) =>
            ValueTuple.Create(self.Item1, self.Item2, thirdMap(self.Item3));

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2, T3>(ValueTuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2, T3>(ValueTuple<T1, T2, T3> self, Action<T1> first, Action<T2> second, Action<T3> third)
        {
            first(self.Item1);
            second(self.Item2);
            third(self.Item3);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T1, T2, T3, S> fold) =>
            fold(state, self.Item1, self.Item2, self.Item3);

        /// <summary>
        /// Tri-fold
        /// </summary>
        [Pure]
        public static S trifold<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold, Func<S, T3, S> thirdFold) =>
            thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3);

        /// <summary>
        /// Tri-fold
        /// </summary>
        [Pure]
        public static S trifoldBack<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T3, S> firstFold, Func<S, T2, S> secondFold, Func<S, T1, S> thirdFold) =>
            thirdFold(secondFold(firstFold(state, self.Item3), self.Item2), self.Item1);
    }
}