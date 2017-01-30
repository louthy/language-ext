using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Append an extra item to the tuple
        /// </summary>
        [Pure]
        public static Tuple<A, B, C, D, E> add<A, B, C, D, E>(Tuple<A, B, C, D> self, E fifth) =>
            Tuple(self.Item1, self.Item2, self.Item3, self.Item4, fifth);

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static Tuple<A, B, C, D> append<SemiA, SemiB, SemiC, SemiD, A, B, C, D>(Tuple<A, B, C, D> a, Tuple<A, B, C, D> b)
            where SemiA : struct, Semigroup<A>
            where SemiB : struct, Semigroup<B>
            where SemiC : struct, Semigroup<C>
            where SemiD : struct, Semigroup<D>
            =>
            Tuple(default(SemiA).Append(a.Item1, b.Item1),
                  default(SemiB).Append(a.Item2, b.Item2),
                  default(SemiC).Append(a.Item3, b.Item3),
                  default(SemiD).Append(a.Item4, b.Item4));

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static A append<SemiA, A>(Tuple<A, A, A, A> a)
            where SemiA : struct, Semigroup<A> =>
            default(SemiA).Append(a.Item1,
                default(SemiA).Append(a.Item2,
                    default(SemiA).Append(a.Item3, a.Item4)));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static Tuple<A, B, C, D> concat<MonoidA, MonoidB, MonoidC, MonoidD, A, B, C, D>(Tuple<A, B, C, D> a, Tuple<A, B, C, D> b)
            where MonoidA : struct, Monoid<A>
            where MonoidB : struct, Monoid<B>
            where MonoidC : struct, Monoid<C>
            where MonoidD : struct, Monoid<D> =>
            Tuple(mconcat<MonoidA, A>(a.Item1, b.Item1),
                  mconcat<MonoidB, B>(a.Item2, b.Item2),
                  mconcat<MonoidC, C>(a.Item3, b.Item3),
                  mconcat<MonoidD, D>(a.Item4, b.Item4));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static A concat<MonoidA, A>(Tuple<A, A, A, A> a)
            where MonoidA : struct, Monoid<A> =>
            mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4);

        /// <summary>
        /// Take the first item
        /// </summary>
        [Pure]
        public static A head<A, B, C, D>(Tuple<A, B, C, D> self) =>
            self.Item1;

        /// <summary>
        /// Take the last item
        /// </summary>
        [Pure]
        public static D last<A, B, C, D>(Tuple<A, B, C, D> self) =>
            self.Item4;

        /// <summary>
        /// Take the second item onwards and build a new tuple
        /// </summary>
        [Pure]
        public static Tuple<B, C, D> tail<A, B, C, D>(Tuple<A, B, C, D> self) =>
            Tuple(self.Item2, self.Item3, self.Item4);

        /// <summary>
        /// Sum of the items
        /// </summary>
        [Pure]
        public static A sum<NUM, A>(Tuple<A, A, A, A> self)
            where NUM : struct, Num<A> =>
            TypeClass.sum<NUM, FoldTuple<A>, Tuple<A, A, A, A>, A>(self);

        /// <summary>
        /// Product of the items
        /// </summary>
        [Pure]
        public static A product<NUM, A>(Tuple<A, A, A, A> self)
            where NUM : struct, Num<A> =>
            TypeClass.product<NUM, FoldTuple<A>, Tuple<A, A, A, A>, A>(self);

        /// <summary>
        /// One of the items matches the value passed
        /// </summary>
        [Pure]
        public static bool contains<EQ, A>(Tuple<A, A, A, A> self, A value)
            where EQ : struct, Eq<A> =>
            default(EQ).Equals(self.Item1, value) ||
            default(EQ).Equals(self.Item2, value) ||
            default(EQ).Equals(self.Item3, value) ||
            default(EQ).Equals(self.Item4, value);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, C, D, R>(Tuple<A, B, C, D> self, Func<Tuple<A, B, C, D>, R> map) =>
            map(self);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, C, D, R>(Tuple<A, B, C, D> self, Func<A, B, C, D, R> map) =>
            map(self.Item1, self.Item2, self.Item3, self.Item4);

        /// <summary>
        /// Tri-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<W, X, Y, Z> map<A, B, C, D, W, X, Y, Z>(Tuple<A, B, C, D> self, Func<A, W> firstMap, Func<B, X> secondMap, Func<C, Y> thirdMap, Func<D, Z> fourthMap) =>
            Tuple(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4));

        /// <summary>
        /// First item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<R1, B, C, D> mapFirst<A, B, C, D, R1>(Tuple<A, B, C, D> self, Func<A, R1> firstMap) =>
            Tuple(firstMap(self.Item1), self.Item2, self.Item3, self.Item4);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<A, R2, C, D> mapSecond<A, B, C, D, R2>(Tuple<A, B, C, D> self, Func<B, R2> secondMap) =>
            Tuple(self.Item1, secondMap(self.Item2), self.Item3, self.Item4);

        /// <summary>
        /// Third item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<A, B, R3, D> mapThird<A, B, C, D, R3>(Tuple<A, B, C, D> self, Func<C, R3> thirdMap) =>
            Tuple(self.Item1, self.Item2, thirdMap(self.Item3), self.Item4);

        /// <summary>
        /// Fourth item-map to tuple
        /// </summary>
        [Pure]
        public static Tuple<A, B, C, R4> mapFourth<A, B, C, D, R4>(Tuple<A, B, C, D> self, Func<D, R4> fourthMap) =>
            Tuple(self.Item1, self.Item2, self.Item3, fourthMap(self.Item4));

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<A, B, C, D>(Tuple<A, B, C, D> self, Action<A, B, C, D> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<A, B, C, D>(Tuple<A, B, C, D> self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth)
        {
            first(self.Item1);
            second(self.Item2);
            third(self.Item3);
            fourth(self.Item4);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<A, B, C, D, S>(Tuple<A, B, C, D> self, S state, Func<S, A, B, C, D, S> fold) =>
            fold(state, self.Item1, self.Item2, self.Item3, self.Item4);

        /// <summary>
        /// Quad-fold
        /// </summary>
        [Pure]
        public static S quadFold<A, B, C, D, S>(Tuple<A, B, C, D> self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold) =>
            fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4);

        /// <summary>
        /// Quad-fold back
        /// </summary>
        [Pure]
        public static S quadFoldBack<A, B, C, D, S>(Tuple<A, B, C, D> self, S state, Func<S, D, S> firstFold, Func<S, C, S> secondFold, Func<S, B, S> thirdFold, Func<S, A, S> fourthFold) =>
            fourthFold(thirdFold(secondFold(firstFold(state, self.Item4), self.Item3), self.Item2), self.Item1);
    }
}