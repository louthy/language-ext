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
        public static ValueTuple<T1, T2, T3> ad<T1, T2, T3>(ValueTuple<T1, T2> self, T3 third) =>
            self.Add(third);

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static ValueTuple<A, B> append<SemiA, SemiB, A, B>(ValueTuple<A, B> a, ValueTuple<A, B> b)
            where SemiA : struct, Semigroup<A>
            where SemiB : struct, Semigroup<B> =>
            (default(SemiA).Append(a.Item1, b.Item1),
             default(SemiB).Append(a.Item2, b.Item2));

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static A append<SemiA, A>(ValueTuple<A, A> a)
            where SemiA : struct, Semigroup<A> =>
            default(SemiA).Append(a.Item1, a.Item2);

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static ValueTuple<A, B> concat<MonoidA, MonoidB, A, B>(ValueTuple<A, B> a, ValueTuple<A, B> b)
            where MonoidA : struct, Monoid<A>
            where MonoidB : struct, Monoid<B> =>
            (mconcat<MonoidA, A>(a.Item1, b.Item1),
             mconcat<MonoidB, B>(a.Item2, b.Item2));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static A concat<MonoidA, A>(ValueTuple<A, A> a)
            where MonoidA : struct, Monoid<A> =>
            mconcat<MonoidA, A>(a.Item1, a.Item2);

        /// <summary>
        /// Take the first item
        /// </summary>
        [Pure]
        public static T1 head<T1, T2>(ValueTuple<T1, T2> self) =>
            self.Item1;

        /// <summary>
        /// Take the last item
        /// </summary>
        [Pure]
        public static T2 last<T1, T2>(ValueTuple<T1, T2> self) =>
            self.Item2;

        /// <summary>
        /// Take the second item onwards and build a new tuple
        /// </summary>
        [Pure]
        public static ValueTuple<T2> tail<T1, T2>(ValueTuple<T1, T2> self) =>
            VTuple(self.Item2);

        /// <summary>
        /// Sum of the items
        /// </summary>
        [Pure]
        public static A sum<NUM, A>(this ValueTuple<A, A> self)
            where NUM : struct, Num<A> =>
            default(NUM).Plus(self.Item1, self.Item2);

        /// <summary>
        /// Product of the items
        /// </summary>
        [Pure]
        public static A product<NUM, A>(this ValueTuple<A, A> self)
            where NUM : struct, Num<A> =>
            default(NUM).Product(self.Item1, self.Item2);

        /// <summary>
        /// One of the items matches the value passed
        /// </summary>
        [Pure]
        public static bool contains<EQ, A>(this ValueTuple<A, A> self, A value)
            where EQ : struct, Eq<A> =>
            default(EQ).Equals(self.Item1, value) ||
            default(EQ).Equals(self.Item2, value);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, R>(ValueTuple<A, B> self, Func<ValueTuple<A, B>, R> map) =>
            map(self);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, R>(ValueTuple<A, B> self, Func<A, B, R> map) =>
            map(self.Item1, self.Item2);

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
        public static Unit iter<T1, T2>(ValueTuple<T1, T2> self, Action<T1, T2> func)
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