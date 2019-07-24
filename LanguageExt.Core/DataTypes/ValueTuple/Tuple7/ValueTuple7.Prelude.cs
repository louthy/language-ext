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
        public static (A, B, C, D, E, F, G, H) add<A, B, C, D, E, F, G, H>((A, B, C, D, E, F, G) self, H eighth) =>
            (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7, eighth);

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static (A, B, C, D, E, F, G) append<SemiA, SemiB, SemiC, SemiD, SemiE, SemiF, SemiG, A, B, C, D, E, F, G>((A, B, C, D, E, F, G) a, (A, B, C, D, E, F, G) b)
            where SemiA : struct, Semigroup<A>
            where SemiB : struct, Semigroup<B>
            where SemiC : struct, Semigroup<C>
            where SemiD : struct, Semigroup<D>
            where SemiE : struct, Semigroup<E>
            where SemiF : struct, Semigroup<F>
            where SemiG : struct, Semigroup<G>
            =>
            (default(SemiA).Append(a.Item1, b.Item1),
             default(SemiB).Append(a.Item2, b.Item2),
             default(SemiC).Append(a.Item3, b.Item3),
             default(SemiD).Append(a.Item4, b.Item4),
             default(SemiE).Append(a.Item5, b.Item5),
             default(SemiF).Append(a.Item6, b.Item6),
             default(SemiG).Append(a.Item7, b.Item7));

        /// <summary>
        /// Semigroup append
        /// </summary>
        [Pure]
        public static A append<SemiA, A>(ValueTuple<A, A, A, A, A, A, A> a)
            where SemiA : struct, Semigroup<A> =>
            default(SemiA).Append(a.Item1,
                default(SemiA).Append(a.Item2,
                    default(SemiA).Append(a.Item3,
                        default(SemiA).Append(a.Item4,
                            default(SemiA).Append(a.Item5,
                                default(SemiA).Append(a.Item6, a.Item7))))));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static (A, B, C, D, E, F, G) concat<MonoidA, MonoidB, MonoidC, MonoidD, MonoidE, MonoidF, MonoidG, A, B, C, D, E, F, G>((A, B, C, D, E, F, G) a, (A, B, C, D, E, F, G) b)
            where MonoidA : struct, Monoid<A>
            where MonoidB : struct, Monoid<B>
            where MonoidC : struct, Monoid<C>
            where MonoidD : struct, Monoid<D>
            where MonoidE : struct, Monoid<E>
            where MonoidF : struct, Monoid<F>
            where MonoidG : struct, Monoid<G> =>
            (mconcat<MonoidA, A>(a.Item1, b.Item1),
             mconcat<MonoidB, B>(a.Item2, b.Item2),
             mconcat<MonoidC, C>(a.Item3, b.Item3),
             mconcat<MonoidD, D>(a.Item4, b.Item4),
             mconcat<MonoidE, E>(a.Item5, b.Item5),
             mconcat<MonoidF, F>(a.Item6, b.Item6),
             mconcat<MonoidG, G>(a.Item7, b.Item7));

        /// <summary>
        /// Monoid concat
        /// </summary>
        [Pure]
        public static A concat<MonoidA, A>(ValueTuple<A, A, A, A, A, A, A> a)
            where MonoidA : struct, Monoid<A> =>
            mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6, a.Item7);

        /// <summary>
        /// Take the first item
        /// </summary>
        [Pure]
        public static A head<A, B, C, D, E, F, G>((A, B, C, D, E, F, G) self) =>
            self.Item1;

        /// <summary>
        /// Take the last item
        /// </summary>
        [Pure]
        public static G last<A, B, C, D, E, F, G>((A, B, C, D, E, F, G) self) =>
            self.Item7;

        /// <summary>
        /// Take the second item onwards and build a new tuple
        /// </summary>
        [Pure]
        public static (B, C, D, E, F, G) tail<A, B, C, D, E, F, G>((A, B, C, D, E, F, G) self) =>
            (self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Sum of the items
        /// </summary>
        [Pure]
        public static A sum<NUM, A>((A, A, A, A, A, A, A) self)
            where NUM : struct, Num<A> =>
            TypeClass.sum<NUM, FoldTuple<A>, (A, A, A, A, A, A, A), A>(self);

        /// <summary>
        /// Product of the items
        /// </summary>
        [Pure]
        public static A product<NUM, A>((A, A, A, A, A, A, A) self)
            where NUM : struct, Num<A> =>
            TypeClass.product<NUM, FoldTuple<A>, (A, A, A, A, A, A, A), A>(self);

        /// <summary>
        /// One of the items matches the value passed
        /// </summary>
        [Pure]
        public static bool contains<EQ, A>((A, A, A, A, A, A, A) self, A value)
            where EQ : struct, Eq<A> =>
            default(EQ).Equals(self.Item1, value) ||
            default(EQ).Equals(self.Item2, value) ||
            default(EQ).Equals(self.Item3, value) ||
            default(EQ).Equals(self.Item4, value) ||
            default(EQ).Equals(self.Item5, value) ||
            default(EQ).Equals(self.Item6, value) ||
            default(EQ).Equals(self.Item7, value);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, C, D, E, F, G, R>(ValueTuple<A, B, C, D, E, F, G> self, Func<ValueTuple<A, B, C, D, E, F, G>, R> map) =>
            map(self);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public static R map<A, B, C, D, E, F, G, R>(ValueTuple<A, B, C, D, E, F, G> self, Func<A, B, C, D, E, F, G, R> map) =>
            map(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Tri-map to tuple
        /// </summary>
        [Pure]
        public static (T, U, V, W, X, Y, Z) map<A, B, C, D, E, F, G, T, U, V, W, X, Y, Z>((A, B, C, D, E, F, G) self, Func<A, T> firstMap, Func<B, U> secondMap, Func<C, V> thirdMap, Func<D, W> fourthMap, Func<E, X> fifthMap, Func<F, Y> sixthMap, Func<G, Z> seventhMap) =>
            (firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4), fifthMap(self.Item5), sixthMap(self.Item6), seventhMap(self.Item7));

        /// <summary>
        /// First item-map to tuple
        /// </summary>
        [Pure]
        public static (R1, B, C, D, E, F, G) mapFirst<A, B, C, D, E, F, G, R1>((A, B, C, D, E, F, G) self, Func<A, R1> firstMap) =>
            (firstMap(self.Item1), self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Second item-map to tuple
        /// </summary>
        [Pure]
        public static (A, R2, C, D, E, F, G) mapSecond<A, B, C, D, E, F, G, R2>((A, B, C, D, E, F, G) self, Func<B, R2> secondMap) =>
            (self.Item1, secondMap(self.Item2), self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Third item-map to tuple
        /// </summary>
        [Pure]
        public static (A, B, R3, D, E, F, G) mapThird<A, B, C, D, E, F, G, R3>((A, B, C, D, E, F, G) self, Func<C, R3> thirdMap) =>
            (self.Item1, self.Item2, thirdMap(self.Item3), self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Fourth item-map to tuple
        /// </summary>
        [Pure]
        public static (A, B, C, R4, E, F, G) mapFourth<A, B, C, D, E, F, G, R4>((A, B, C, D, E, F, G) self, Func<D, R4> fourthMap) =>
            (self.Item1, self.Item2, self.Item3, fourthMap(self.Item4), self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Fifth item-map to tuple
        /// </summary>
        [Pure]
        public static (A, B, C, D, R5, F, G) mapFifth<A, B, C, D, E, F, G, R5>((A, B, C, D, E, F, G) self, Func<E, R5> fifthMap) =>
            (self.Item1, self.Item2, self.Item3, self.Item4, fifthMap(self.Item5), self.Item6, self.Item7);

        /// <summary>
        /// Sixth item-map to tuple
        /// </summary>
        [Pure]
        public static (A, B, C, D, E, R6, G) mapSixth<A, B, C, D, E, F, G, R6>((A, B, C, D, E, F, G) self, Func<F, R6> sixthMap) =>
            (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, sixthMap(self.Item6), self.Item7);

        /// <summary>
        /// Sixth item-map to tuple
        /// </summary>
        [Pure]
        public static (A, B, C, D, E, F, R7) mapSeventh<A, B, C, D, E, F, G, R7>((A, B, C, D, E, F, G) self, Func<G, R7> seventhMap) =>
            (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, seventhMap(self.Item7));

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<A, B, C, D, E, F, G>((A, B, C, D, E, F, G) self, Action<A, B, C, D, E, F, G> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }

        /// <summary>
        /// Iterate
        /// </summary>
        public static Unit iter<A, B, C, D, E, F, G>((A, B, C, D, E, F, G) self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth, Action<E> fifth, Action<F> sixth, Action<G> seventh)
        {
            first(self.Item1);
            second(self.Item2);
            third(self.Item3);
            fourth(self.Item4);
            fifth(self.Item5);
            sixth(self.Item6);
            seventh(self.Item7);
            return Unit.Default;
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S fold<A, B, C, D, E, F, G, S>((A, B, C, D, E, F, G) self, S state, Func<S, A, B, C, D, E, F, G, S> fold) =>
            fold(state, self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public static S septFold<A, B, C, D, E, F, G, S>((A, B, C, D, E, F, G) self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold, Func<S, E, S> fifthFold, Func<S, F, S> sixthFold, Func<S, G, S> seventhFold) =>
            seventhFold(sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4), self.Item5), self.Item6), self.Item7);

        /// <summary>
        /// Fold back
        /// </summary>
        [Pure]
        public static S septFoldBack<A, B, C, D, E, F, G, S>((A, B, C, D, E, F, G) self, S state, Func<S, G, S> firstFold, Func<S, F, S> secondFold, Func<S, E, S> thirdFold, Func<S, D, S> fourthFold, Func<S, C, S> fifthFold, Func<S, B, S> sixthFold, Func<S, A, S> seventhFold) =>
            seventhFold(sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item7), self.Item6), self.Item5), self.Item4), self.Item3), self.Item2), self.Item1);
    }
}
