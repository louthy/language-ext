using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

public static class Tuple7Extensions
{
    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F, G> Append<SemiA, SemiB, SemiC, SemiD, SemiE, SemiF, SemiG, A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> a, Tuple<A, B, C, D, E, F, G> b)
        where SemiA : Semigroup<A>
        where SemiB : Semigroup<B>
        where SemiC : Semigroup<C>
        where SemiD : Semigroup<D> 
        where SemiE : Semigroup<E>
        where SemiF : Semigroup<F> 
        where SemiG : Semigroup<G> =>
        Tuple(SemiA.Append(a.Item1, b.Item1),
              SemiB.Append(a.Item2, b.Item2),
              SemiC.Append(a.Item3, b.Item3),
              SemiD.Append(a.Item4, b.Item4),
              SemiE.Append(a.Item5, b.Item5),
              SemiF.Append(a.Item6, b.Item6),
              SemiG.Append(a.Item7, b.Item7));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this Tuple<A, A, A, A, A, A, A> a)
        where SemiA : Semigroup<A> =>
        SemiA.Append(a.Item1,
            SemiA.Append(a.Item2,
                SemiA.Append(a.Item3,
                    SemiA.Append(a.Item4,
                        SemiA.Append(a.Item5,
                            SemiA.Append(a.Item6, a.Item7))))));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F, G> Concat<MonoidA, MonoidB, MonoidC, MonoidD, MonoidE, MonoidF, MonoidG, A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> a, Tuple<A, B, C, D, E, F, G> b)
        where MonoidA : Monoid<A>
        where MonoidB : Monoid<B>
        where MonoidC : Monoid<C>
        where MonoidD : Monoid<D>
        where MonoidE : Monoid<E>
        where MonoidF : Monoid<F>
        where MonoidG : Monoid<G> =>
        Tuple(mconcat<MonoidA, A>(a.Item1, b.Item1),
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
    public static A Concat<MonoidA, A>(this Tuple<A, A, A, A, A, A, A> a)
        where MonoidA : Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6, a.Item7);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static G Last<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> self) =>
        self.Item7;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static (B, C, D, E, F, G) Tail<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> self) =>
        (self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this Tuple<A, A, A, A, A, A, A> self)
        where NUM : Num<A> =>
        sum<NUM, FoldTuple<A>, Tuple<A, A, A, A, A, A, A>, A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this Tuple<A, A, A, A, A, A, A> self)
        where NUM : Num<A> =>
        product<NUM, FoldTuple<A>, Tuple<A, A, A, A, A, A, A>, A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this Tuple<A, A, A, A, A, A, A> self, A value)
        where EQ : Eq<A> =>
        EQ.Equals(self.Item1, value) ||
        EQ.Equals(self.Item2, value) ||
        EQ.Equals(self.Item3, value) ||
        EQ.Equals(self.Item4, value) ||
        EQ.Equals(self.Item5, value) ||
        EQ.Equals(self.Item6, value) ||
        EQ.Equals(self.Item7, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, F, G, R>(this Tuple<A, B, C, D, E, F, G> self, Func<Tuple<A, B, C, D, E, F, G>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, F, G, R>(this Tuple<A, B, C, D, E, F, G> self, Func<A, B, C, D, E, F, G, R> map) =>
        map(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<T, U, V, W, X, Y, Z> Map<A, B, C, D, E, F, G, T, U, V, W, X, Y, Z>(this Tuple<A, B, C, D, E, F, G> self, Func<A, T> firstMap, Func<B, U> secondMap, Func<C, V> thirdMap, Func<D, W> fourthMap, Func<E, X> fifthMap, Func<F, Y> sixthMap, Func<G, Z> seventhMap) =>
        Tuple(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4), fifthMap(self.Item5), sixthMap(self.Item6), seventhMap(self.Item7));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R1, B, C, D, E, F, G> MapFirst<A, B, C, D, E, F, G, R1>(this Tuple<A, B, C, D, E, F, G> self, Func<A, R1> firstMap) =>
        Tuple(firstMap(self.Item1), self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, R2, C, D, E, F, G> MapSecond<A, B, C, D, E, F, G, R2>(this Tuple<A, B, C, D, E, F, G> self, Func<B, R2> secondMap) =>
        Tuple(self.Item1, secondMap(self.Item2), self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, R3, D, E, F, G> MapThird<A, B, C, D, E, F, G, R3>(this Tuple<A, B, C, D, E, F, G> self, Func<C, R3> thirdMap) =>
        Tuple(self.Item1, self.Item2, thirdMap(self.Item3), self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Fourth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, R4, E, F, G> MapFourth<A, B, C, D, E, F, G, R4>(this Tuple<A, B, C, D, E, F, G> self, Func<D, R4> fourthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, fourthMap(self.Item4), self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Fifth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, R5, F, G> MapFifth<A, B, C, D, E, F, G, R5>(this Tuple<A, B, C, D, E, F, G> self, Func<E, R5> fifthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, fifthMap(self.Item5), self.Item6, self.Item7);

    /// <summary>
    /// Sixth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, R6, G> MapSixth<A, B, C, D, E, F, G, R6>(this Tuple<A, B, C, D, E, F, G> self, Func<F, R6> sixthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, sixthMap(self.Item6), self.Item7);

    /// <summary>
    /// Sixth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F, R7> MapSeventh<A, B, C, D, E, F, G, R7>(this Tuple<A, B, C, D, E, F, G> self, Func<G, R7> seventhMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, seventhMap(self.Item7));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static Tuple<U, V, W, X, Y, Z> Select<A, B, C, D, E, F, G, U, V, W, X, Y, Z>(this Tuple<A, B, C, D, E, F, G> self, Func<Tuple<A, B, C, D, E, F, G>, Tuple<U, V, W, X, Y, Z>> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> self, Action<A, B, C, D, E, F, G> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth, Action<E> fifth, Action<F> sixth, Action<G> seventh)
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
    public static S Fold<A, B, C, D, E, F, G, S>(this Tuple<A, B, C, D, E, F, G> self, S state, Func<S, A, B, C, D, E, F, G, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S SeptFold<A, B, C, D, E, F, G, S>(this Tuple<A, B, C, D, E, F, G> self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold, Func<S, E, S> fifthFold, Func<S, F, S> sixthFold, Func<S, G, S> seventhFold) =>
        seventhFold(sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4), self.Item5), self.Item6), self.Item7);

    /// <summary>
    /// Fold back
    /// </summary>
    [Pure]
    public static S SeptFoldBack<A, B, C, D, E, F, G, S>(this Tuple<A, B, C, D, E, F, G> self, S state, Func<S, G, S> firstFold, Func<S, F, S> secondFold, Func<S, E, S> thirdFold, Func<S, D, S> fourthFold, Func<S, C, S> fifthFold, Func<S, B, S> sixthFold, Func<S, A, S> seventhFold) =>
        seventhFold(sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item7), self.Item6), self.Item5), self.Item4), self.Item3), self.Item2), self.Item1);
}
