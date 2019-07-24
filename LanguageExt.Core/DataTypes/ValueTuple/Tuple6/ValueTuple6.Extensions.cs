using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

public static class ValueTuple6Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, E, F, G) Add<A, B, C, D, E, F, G>(this (A, B, C, D, E, F) self, G seventh) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, seventh);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static (A, B, C, D, E, F) Append<SemiA, SemiB, SemiC, SemiD, SemiE, SemiF, A, B, C, D, E, F>(this (A, B, C, D, E, F) a, (A, B, C, D, E, F) b)
        where SemiA : struct, Semigroup<A>
        where SemiB : struct, Semigroup<B>
        where SemiC : struct, Semigroup<C>
        where SemiD : struct, Semigroup<D> 
        where SemiE : struct, Semigroup<E>
        where SemiF : struct, Semigroup<F> 
        =>
        (default(SemiA).Append(a.Item1, b.Item1),
         default(SemiB).Append(a.Item2, b.Item2),
         default(SemiC).Append(a.Item3, b.Item3),
         default(SemiD).Append(a.Item4, b.Item4),
         default(SemiE).Append(a.Item5, b.Item5),
         default(SemiF).Append(a.Item6, b.Item6));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this ValueTuple<A, A, A, A, A, A> a)
        where SemiA : struct, Semigroup<A> =>
        default(SemiA).Append(a.Item1,
            default(SemiA).Append(a.Item2,
                default(SemiA).Append(a.Item3,
                    default(SemiA).Append(a.Item4,
                        default(SemiA).Append(a.Item5, a.Item6)))));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static (A, B, C, D, E, F) Concat<MonoidA, MonoidB, MonoidC, MonoidD, MonoidE, MonoidF, A, B, C, D, E, F>(this (A, B, C, D, E, F) a, (A, B, C, D, E, F) b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B>
        where MonoidC : struct, Monoid<C>
        where MonoidD : struct, Monoid<D>
        where MonoidE : struct, Monoid<E>
        where MonoidF : struct, Monoid<F> =>
        (mconcat<MonoidA, A>(a.Item1, b.Item1),
         mconcat<MonoidB, B>(a.Item2, b.Item2),
         mconcat<MonoidC, C>(a.Item3, b.Item3),
         mconcat<MonoidD, D>(a.Item4, b.Item4),
         mconcat<MonoidE, E>(a.Item5, b.Item5),
         mconcat<MonoidF, F>(a.Item6, b.Item6));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A Concat<MonoidA, A>(this ValueTuple<A, A, A, A, A, A> a)
        where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5, a.Item6);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A, B, C, D, E, F>(this (A, B, C, D, E, F) self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static F Last<A, B, C, D, E, F>(this (A, B, C, D, E, F) self) =>
        self.Item6;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static (B, C, D, E, F) Tail<A, B, C, D, E, F>(this(A, B, C, D, E, F) self) =>
        (self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this (A, A, A, A, A, A) self)
        where NUM : struct, Num<A> =>
        TypeClass.sum<NUM, FoldTuple<A>, (A, A, A, A, A, A), A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this(A, A, A, A, A, A) self)
        where NUM : struct, Num<A> =>
        TypeClass.product<NUM, FoldTuple<A>, (A, A, A, A, A, A), A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this (A, A, A, A, A, A) self, A value)
        where EQ : struct, Eq<A> =>
        default(EQ).Equals(self.Item1, value) ||
        default(EQ).Equals(self.Item2, value) ||
        default(EQ).Equals(self.Item3, value) ||
        default(EQ).Equals(self.Item4, value) ||
        default(EQ).Equals(self.Item5, value) ||
        default(EQ).Equals(self.Item6, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, F, R>(this ValueTuple<A, B, C, D, E, F> self, Func<ValueTuple<A, B, C, D, E, F>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, F, R>(this ValueTuple<A, B, C, D, E, F> self, Func<A, B, C, D, E, F, R> map) =>
        map(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static (U, V, W, X, Y, Z) Map<A, B, C, D, E, F, U, V, W, X, Y, Z>(this(A, B, C, D, E, F) self, Func<A, U> firstMap, Func<B, V> secondMap, Func<C, W> thirdMap, Func<D, X> fourthMap, Func<E, Y> fifthMap, Func<F, Z> sixthMap) =>
        (firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4), fifthMap(self.Item5), sixthMap(self.Item6));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static (R1, B, C, D, E, F) MapFirst<A, B, C, D, E, F, R1>(this (A, B, C, D, E, F) self, Func<A, R1> firstMap) =>
        (firstMap(self.Item1), self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static (A, R2, C, D, E, F) MapSecond<A, B, C, D, E, F, R2>(this (A, B, C, D, E, F) self, Func<B, R2> secondMap) =>
        (self.Item1, secondMap(self.Item2), self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, R3, D, E, F) MapThird<A, B, C, D, E, F, R3>(this (A, B, C, D, E, F) self, Func<C, R3> thirdMap) =>
        (self.Item1, self.Item2, thirdMap(self.Item3), self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Fourth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, R4, E, F) MapFourth<A, B, C, D, E, F, R4>(this(A, B, C, D, E, F) self, Func<D, R4> fourthMap) =>
        (self.Item1, self.Item2, self.Item3, fourthMap(self.Item4), self.Item5, self.Item6);

    /// <summary>
    /// Fifth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, R5, F) MapFifth<A, B, C, D, E, F, R5>(this(A, B, C, D, E, F) self, Func<E, R5> fifthMap) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, fifthMap(self.Item5), self.Item6);

    /// <summary>
    /// Sixth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, E, R6) MapSixth<A, B, C, D, E, F, R6>(this(A, B, C, D, E, F) self, Func<F, R6> sixthMap) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, sixthMap(self.Item6));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static (U, V, W, X, Y, Z) Select<A, B, C, D, E, F, U, V, W, X, Y, Z>(this (A, B, C, D, E, F) self, Func<(A, B, C, D, E, F), (U, V, W, X, Y, Z)> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E, F>(this (A, B, C, D, E, F) self, Action<A, B, C, D, E, F> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E, F>(this (A, B, C, D, E, F) self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth, Action<E> fifth, Action<F> sixth)
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
    public static S Fold<A, B, C, D, E, F, S>(this (A, B, C, D, E, F) self, S state, Func<S, A, B, C, D, E, F, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S SextFold<A, B, C, D, E, F, S>(this(A, B, C, D, E, F) self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold, Func<S, E, S> fifthFold, Func<S, F, S> sixthFold) =>
        sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4), self.Item5), self.Item6);

    /// <summary>
    /// Fold back
    /// </summary>
    [Pure]
    public static S SextFoldBack<A, B, C, D, E, F, S>(this (A, B, C, D, E, F) self, S state, Func<S, F, S> firstFold, Func<S, E, S> secondFold, Func<S, D, S> thirdFold, Func<S, C, S> fourthFold, Func<S, B, S> fifthFold, Func<S, A, S> sixthFold) =>
        sixthFold(fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item6), self.Item5), self.Item4), self.Item3), self.Item2), self.Item1);
}