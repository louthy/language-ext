using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append an extra item to the tuple
        /// </summary>
        [Pure]
        public static Tuple<T1, T2, T3> append<T1, T2, T3>(Tuple<T1, T2> self, T3 third) =>
            self.Append(third);

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static Tuple<A, B> append<SemiA, SemiB, A, B>(Tuple<A, B> a, Tuple<A, B> b)
            where SemiA : struct, Semigroup<A>
            where SemiB : struct, Semigroup<B> =>
            Tuple(
                default(SemiA).Append(a.Item1, b.Item1),
                default(SemiB).Append(a.Item2, b.Item2));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static Tuple<A, B> concat<MonoidA, MonoidB, A, B>(Tuple<A, B> a, Tuple<A, B> b)
            where MonoidA : struct, Monoid<A>
            where MonoidB : struct, Monoid<B> =>
            Tuple(
                mconcat<MonoidA, A>(a.Item1, b.Item1),
                mconcat<MonoidB, B>(a.Item2, b.Item2));

        /// <summary>
        /// Take the first item
        /// </summary>
        [Pure]
        public static T1 head<T1, T2>(Tuple<T1, T2> self) =>
            self.Item1;

        /// <summary>
        /// Take the last item
        /// </summary>
        [Pure]
        public static T2 last<T1, T2>(Tuple<T1, T2> self) =>
            self.Item2;

        /// <summary>
        /// Take the second item onwards and build a new tuple
        /// </summary>
        [Pure]
        public static Tuple<T2> tail<T1, T2>(Tuple<T1, T2> self) =>
            Tuple(self.Item2);

        /// <summary>
        /// Map to R
        /// </summary>
        [Pure]
        public static R map<T1, T2, R>(Tuple<T1, T2> self, Func<T1, T2, R> map) =>
            self.Map(map);

        /// <summary>
        /// Map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, R2> map<T1, T2, R1, R2>(Tuple<T1, T2> self, Func<Tuple<T1, T2>, Tuple<R1, R2>> map) =>
            self.Map(map);

        /// <summary>
        /// Bi-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, R2> bimap<T1, T2, R1, R2>(Tuple<T1, T2> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap) =>
            self.BiMap(firstMap, secondMap);

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2>(Tuple<T1, T2> self, Action<T1> first, Action<T2> second) =>
            self.Iter(first, second);

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<T1, T2>(Tuple<T1, T2> self, Action<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<T1, T2, S>(Tuple<T1, T2> self, S state, Func<S, T1, T2, S> fold) =>
            self.Fold(state, fold);

        /// <summary>
        /// Bi-fold
        /// </summary>
        [Pure]
        public static S bifold<T1, T2, S>(Tuple<T1, T2> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold) =>
            self.BiFold(state, firstFold, secondFold);

        /// <summary>
        /// Bi-fold back
        /// </summary>
        [Pure]
        public static S bifoldBack<T1, T2, S>(Tuple<T1, T2> self, S state, Func<S, T2, S> firstFold, Func<S, T1, S> secondFold) =>
            self.BiFoldBack(state, firstFold, secondFold);
    }
}