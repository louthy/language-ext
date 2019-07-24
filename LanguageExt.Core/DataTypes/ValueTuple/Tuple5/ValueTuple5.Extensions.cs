using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

public static class ValueTuple5Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, E, F) Add<A, B, C, D, E, F>(this (A, B, C, D, E) self, F sixth) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, sixth);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static (A, B, C, D, E) Append<SemiA, SemiB, SemiC, SemiD, SemiE, A, B, C, D, E>(this (A, B, C, D, E) a, (A, B, C, D, E) b)
        where SemiA : struct, Semigroup<A>
        where SemiB : struct, Semigroup<B>
        where SemiC : struct, Semigroup<C>
        where SemiD : struct, Semigroup<D> 
        where SemiE : struct, Semigroup<E> =>
        (default(SemiA).Append(a.Item1, b.Item1),
         default(SemiB).Append(a.Item2, b.Item2),
         default(SemiC).Append(a.Item3, b.Item3),
         default(SemiD).Append(a.Item4, b.Item4),
         default(SemiE).Append(a.Item5, b.Item5));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this ValueTuple<A, A, A, A, A> a)
        where SemiA : struct, Semigroup<A> =>
        default(SemiA).Append(a.Item1,
            default(SemiA).Append(a.Item2,
                default(SemiA).Append(a.Item3,
                    default(SemiA).Append(a.Item4, a.Item5))));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static (A, B, C, D, E) Concat<MonoidA, MonoidB, MonoidC, MonoidD, MonoidE, A, B, C, D, E>(this (A, B, C, D, E) a, (A, B, C, D, E) b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B>
        where MonoidC : struct, Monoid<C>
        where MonoidD : struct, Monoid<D>
        where MonoidE : struct, Monoid<E> =>
        (mconcat<MonoidA, A>(a.Item1, b.Item1),
         mconcat<MonoidB, B>(a.Item2, b.Item2),
         mconcat<MonoidC, C>(a.Item3, b.Item3),
         mconcat<MonoidD, D>(a.Item4, b.Item4),
         mconcat<MonoidE, E>(a.Item5, b.Item5));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A Concat<MonoidA, A>(this ValueTuple<A, A, A, A, A> a)
        where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A, B, C, D, E>(this (A, B, C, D, E) self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static E Last<A, B, C, D, E>(this (A, B, C, D, E) self) =>
        self.Item5;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static (B, C, D, E) Tail<A, B, C, D, E>(this(A, B, C, D, E) self) =>
        (self.Item2, self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this (A, A, A, A, A) self)
        where NUM : struct, Num<A> =>
        TypeClass.sum<NUM, FoldTuple<A>, (A, A, A, A, A), A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this(A, A, A, A, A) self)
        where NUM : struct, Num<A> =>
        TypeClass.product<NUM, FoldTuple<A>, (A, A, A, A, A), A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this (A, A, A, A, A) self, A value)
        where EQ : struct, Eq<A> =>
        default(EQ).Equals(self.Item1, value) ||
        default(EQ).Equals(self.Item2, value) ||
        default(EQ).Equals(self.Item3, value) ||
        default(EQ).Equals(self.Item4, value) ||
        default(EQ).Equals(self.Item5, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, R>(this ValueTuple<A, B, C, D, E> self, Func<ValueTuple<A, B, C, D, E>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, E, R>(this ValueTuple<A, B, C, D, E> self, Func<A, B, C, D, E, R> map) =>
        map(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static (V, W, X, Y, Z) Map<A, B, C, D, E, V, W, X, Y, Z>(this(A, B, C, D, E) self, Func<A, V> firstMap, Func<B, W> secondMap, Func<C, X> thirdMap, Func<D, Y> fourthMap, Func<E, Z> fifthMap) =>
        (firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4), fifthMap(self.Item5));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static (R1, B, C, D, E) MapFirst<A, B, C, D, E, R1>(this (A, B, C, D, E) self, Func<A, R1> firstMap) =>
        (firstMap(self.Item1), self.Item2, self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static (A, R2, C, D, E) MapSecond<A, B, C, D, E, R2>(this (A, B, C, D, E) self, Func<B, R2> secondMap) =>
        (self.Item1, secondMap(self.Item2), self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, R3, D, E) MapThird<A, B, C, D, E, R3>(this (A, B, C, D, E) self, Func<C, R3> thirdMap) =>
        (self.Item1, self.Item2, thirdMap(self.Item3), self.Item4, self.Item5);

    /// <summary>
    /// Fourth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, R4, E) MapFourth<A, B, C, D, E, R4>(this(A, B, C, D, E) self, Func<D, R4> fourthMap) =>
        (self.Item1, self.Item2, self.Item3, fourthMap(self.Item4), self.Item5);

    /// <summary>
    /// Fifth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, R5) MapFifth<A, B, C, D, E, R5>(this(A, B, C, D, E) self, Func<E, R5> fifthMap) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, fifthMap(self.Item5));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static (V, W, X, Y, Z) Select<A, B, C, D, E, V, W, X, Y, Z>(this (A, B, C, D, E) self, Func<(A, B, C, D, E), (V, W, X, Y, Z)> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E>(this (A, B, C, D, E) self, Action<A, B, C, D, E> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D, E>(this (A, B, C, D, E) self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth, Action<E> fifth)
    {
        first(self.Item1);
        second(self.Item2);
        third(self.Item3);
        fourth(self.Item4);
        fifth(self.Item5);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S Fold<A, B, C, D, E, S>(this (A, B, C, D, E) self, S state, Func<S, A, B, C, D, E, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S QuintFold<A, B, C, D, E, S>(this(A, B, C, D, E) self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold, Func<S, E, S> fifthFold) =>
        fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4), self.Item5);

    /// <summary>
    /// Fold back
    /// </summary>
    [Pure]
    public static S QuintFoldBack<A, B, C, D, E, S>(this (A, B, C, D, E) self, S state, Func<S, E, S> firstFold, Func<S, D, S> secondFold, Func<S, C, S> thirdFold, Func<S, B, S> fourthFold, Func<S, A, S> fifthFold) =>
        fifthFold(fourthFold(thirdFold(secondFold(firstFold(state, self.Item5), self.Item4), self.Item3), self.Item2), self.Item1);
}