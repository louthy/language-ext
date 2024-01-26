using System;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F, G> add<A, B, C, D, E, F, G>(Tuple<A, B, C, D, E, F> self, G seventh) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, seventh);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F> append<SemiA, SemiB, SemiC, SemiD, SemiE, SemiF, A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> a, Tuple<A, B, C, D, E, F> b)
        where SemiA : Semigroup<A>
        where SemiB : Semigroup<B>
        where SemiC : Semigroup<C>
        where SemiD : Semigroup<D>
        where SemiE : Semigroup<E>
        where SemiF : Semigroup<F>
        =>
            Tuple(SemiA.Append(a.Item1, b.Item1),
                  SemiB.Append(a.Item2, b.Item2),
                  SemiC.Append(a.Item3, b.Item3),
                  SemiD.Append(a.Item4, b.Item4),
                  SemiE.Append(a.Item5, b.Item5),
                  SemiF.Append(a.Item6, b.Item6));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A append<SemiA, A>(Tuple<A, A, A, A, A, A> a)
        where SemiA : Semigroup<A> =>
        SemiA.Append(a.Item1,
                     SemiA.Append(a.Item2,
                                  SemiA.Append(a.Item3,
                                               SemiA.Append(a.Item4,
                                                            SemiA.Append(a.Item5, a.Item6)))));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, F> concat<MonoidA, MonoidB, MonoidC, MonoidD, MonoidE, MonoidF, A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> a, Tuple<A, B, C, D, E, F> b)
        where MonoidA : Monoid<A>
        where MonoidB : Monoid<B>
        where MonoidC : Monoid<C>
        where MonoidD : Monoid<D>
        where MonoidE : Monoid<E>
        where MonoidF : Monoid<F> =>
        Tuple(mconcat<MonoidA, A>(a.Item1, b.Item1),
              mconcat<MonoidB, B>(a.Item2, b.Item2),
              mconcat<MonoidC, C>(a.Item3, b.Item3),
              mconcat<MonoidD, D>(a.Item4, b.Item4),
              mconcat<MonoidE, E>(a.Item5, b.Item5),
              mconcat<MonoidF, F>(a.Item6, b.Item6));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A concat<MonoidA, A>(Tuple<A, A, A, A, A, A> a)
        where MonoidA : Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A head<A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static F last<A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> self) =>
        self.Item6;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static Tuple<B, C, D, E, F> tail<A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> self) =>
        Tuple(self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A sum<NUM, A>(Tuple<A, A, A, A, A, A> self)
        where NUM : Num<A> =>
        TypeClass.sum<NUM, FoldTuple<A>, Tuple<A, A, A, A, A, A>, A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A product<NUM, A>(Tuple<A, A, A, A, A, A> self)
        where NUM : Num<A> =>
        TypeClass.product<NUM, FoldTuple<A>, Tuple<A, A, A, A, A, A>, A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool contains<EQ, A>(Tuple<A, A, A, A, A, A> self, A value)
        where EQ : Eq<A> =>
        EQ.Equals(self.Item1, value) ||
        EQ.Equals(self.Item2, value) ||
        EQ.Equals(self.Item3, value) ||
        EQ.Equals(self.Item4, value) ||
        EQ.Equals(self.Item5, value) ||
        EQ.Equals(self.Item6, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R map<A, B, C, D, E, F, R>(Tuple<A, B, C, D, E, F> self, Func<Tuple<A, B, C, D, E, F>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R map<A, B, C, D, E, F, R>(Tuple<A, B, C, D, E, F> self, Func<A, B, C, D, E, F, R> map) =>
        map(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<U, V, W, X, Y, Z> map<A, B, C, D, E, F, U, V, W, X, Y, Z>(Tuple<A, B, C, D, E, F> self, Func<A, U> firstMap, Func<B, V> secondMap, Func<C, W> thirdMap, Func<D, X> fourthMap, Func<E, Y> fifthMap, Func<F, Z> sixthMap) =>
        Tuple(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4), fifthMap(self.Item5), sixthMap(self.Item6));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R1, B, C, D, E, F> mapFirst<A, B, C, D, E, F, R1>(Tuple<A, B, C, D, E, F> self, Func<A, R1> firstMap) =>
        Tuple(firstMap(self.Item1), self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, R2, C, D, E, F> mapSecond<A, B, C, D, E, F, R2>(Tuple<A, B, C, D, E, F> self, Func<B, R2> secondMap) =>
        Tuple(self.Item1, secondMap(self.Item2), self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, R3, D, E, F> mapThird<A, B, C, D, E, F, R3>(Tuple<A, B, C, D, E, F> self, Func<C, R3> thirdMap) =>
        Tuple(self.Item1, self.Item2, thirdMap(self.Item3), self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Fourth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, R4, E, F> mapFourth<A, B, C, D, E, F, R4>(Tuple<A, B, C, D, E, F> self, Func<D, R4> fourthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, fourthMap(self.Item4), self.Item5, self.Item6);

    /// <summary>
    /// Fifth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, R5, F> mapFifth<A, B, C, D, E, F, R5>(Tuple<A, B, C, D, E, F> self, Func<E, R5> fifthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, fifthMap(self.Item5), self.Item6);

    /// <summary>
    /// Sixth item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C, D, E, R6> mapSixth<A, B, C, D, E, F, R6>(Tuple<A, B, C, D, E, F> self, Func<F, R6> sixthMap) =>
        Tuple(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, sixthMap(self.Item6));

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit iter<A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> self, Action<A, B, C, D, E, F> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit iter<A, B, C, D, E, F>(Tuple<A, B, C, D, E, F> self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth, Action<E> fifth, Action<F> sixth)
    {
        first(self.Item1);
        second(self.Item2);
        third(self.Item3);
        fourth(self.Item4);
        fifth(self.Item5);
        sixth(self.Item6);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S fold<A, B, C, D, E, F, S>(Tuple<A, B, C, D, E, F> self, S state, Func<S, A, B, C, D, E, F, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S sextFold<A, B, C, D, E, F, S>(Tuple<A, B, C, D, E, F> self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold, Func<S, E, S> fifthFold, Func<S, F, S> sixthFold) =>
        sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4), self.Item5), self.Item6);

    /// <summary>
    /// Fold back
    /// </summary>
    [Pure]
    public static S sextFoldBack<A, B, C, D, E, F, S>(Tuple<A, B, C, D, E, F> self, S state, Func<S, F, S> firstFold, Func<S, E, S> secondFold, Func<S, D, S> thirdFold, Func<S, C, S> fourthFold, Func<S, B, S> fifthFold, Func<S, A, S> sixthFold) =>
        sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item6), self.Item5), self.Item4), self.Item3), self.Item2), self.Item1);
}
