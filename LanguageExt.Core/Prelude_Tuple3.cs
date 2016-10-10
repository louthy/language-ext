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
        public static Tuple<T1, T2, T3, T4> append<T1, T2, T3, T4>(this Tuple<T1, T2, T3> self, T4 fourth) =>
            Tuple(self.Item1, self.Item2, self.Item3, fourth);

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static int sum(Tuple<int, int, int> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static double sum(Tuple<double, double, double> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static float sum(Tuple<float, float, float> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Sum
        /// </summary>
        [Pure]
        public static decimal sum(Tuple<decimal, decimal, decimal> self) =>
            self.Item1 + self.Item2 + self.Item3;

        /// <summary>
        /// Map to R
        /// </summary>
        [Pure]
        public static R map<T1, T2, T3, R>(Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> map) =>
            map(self.Item1, self.Item2, self.Item3);

        /// <summary>
        /// Map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, R2, R3> map<T1, T2, T3, R1, R2, R3>(Tuple<T1, T2, T3> self, Func<Tuple<T1, T2, T3>, Tuple<R1, R2, R3>> map) =>
            map(self);

        /// <summary>
        /// Tri-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, R2, R3> trimap<T1, T2, T3, R1, R2, R3>(Tuple<T1, T2, T3> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap, Func<T3, R3> thirdMap) =>
            Tuple(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3));

        /// <summary>
        /// First item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, T2, T3> mapFirst<T1, T2, T3, R1>(Tuple<T1, T2, T3> self, Func<T1, R1> firstMap) =>
            Tuple(firstMap(self.Item1), self.Item2, self.Item3);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<T1, R2, T3> mapSecond<T1, T2, T3, R2>(Tuple<T1, T2, T3> self, Func<T2, R2> secondMap) =>
            Tuple(self.Item1, secondMap(self.Item2), self.Item3);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<T1, T2, R3> mapThird<T1, T2, T3, R3>(Tuple<T1, T2, T3> self, Func<T3, R3> thirdMap) =>
            Tuple(self.Item1, self.Item2, thirdMap(self.Item3));

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2, T3>(Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2, T3>(Tuple<T1, T2, T3> self, Action<T1> first, Action<T2> second, Action<T3> third)
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
        public static S fold<T1, T2, T3, S>(Tuple<T1, T2, T3> self, S state, Func<S, T1, T2, T3, S> fold) =>
            fold(state, self.Item1, self.Item2, self.Item3);

        /// <summary>
        /// Tri-fold
        /// </summary>
        [Pure]
        public static S trifold<T1, T2, T3, S>(Tuple<T1, T2, T3> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold, Func<S, T3, S> thirdFold) =>
            thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3);

        /// <summary>
        /// Tri-fold
        /// </summary>
        [Pure]
        public static S trifoldBack<T1, T2, T3, S>(Tuple<T1, T2, T3> self, S state, Func<S, T3, S> firstFold, Func<S, T2, S> secondFold, Func<S, T1, S> thirdFold) =>
            thirdFold(secondFold(firstFold(state, self.Item3), self.Item2), self.Item1);
    }
}