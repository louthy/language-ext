using System;
using LanguageExt;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

public static class ValueTuple4Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static (A, B, C, D, E) Add<A, B, C, D, E>(this (A, B, C, D) self, E fifth) =>
        (self.Item1, self.Item2, self.Item3, self.Item4, fifth);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A, B, C, D>(this (A, B, C, D) self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static D Last<A, B, C, D>(this (A, B, C, D) self) =>
        self.Item4;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static (B, C, D) Tail<A, B, C, D>(this(A, B, C, D) self) =>
        (self.Item2, self.Item3, self.Item4);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this (A, A, A, A) self, A value)
        where EQ : Eq<A> =>
        EQ.Equals(self.Item1, value) ||
        EQ.Equals(self.Item2, value) ||
        EQ.Equals(self.Item3, value) ||
        EQ.Equals(self.Item4, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, R>(this ValueTuple<A, B, C, D> self, Func<ValueTuple<A, B, C, D>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, D, R>(this ValueTuple<A, B, C, D> self, Func<A, B, C, D, R> map) =>
        map(self.Item1, self.Item2, self.Item3, self.Item4);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static (W, X, Y, Z) Map<A, B, C, D, W, X, Y, Z>(this(A, B, C, D) self, Func<A, W> firstMap, Func<B, X> secondMap, Func<C, Y> thirdMap, Func<D, Z> fourthMap) =>
        (firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3), fourthMap(self.Item4));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static (R1, B, C, D) MapFirst<A, B, C, D, R1>(this (A, B, C, D) self, Func<A, R1> firstMap) =>
        (firstMap(self.Item1), self.Item2, self.Item3, self.Item4);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static (A, R2, C, D) MapSecond<A, B, C, D, R2>(this (A, B, C, D) self, Func<B, R2> secondMap) =>
        (self.Item1, secondMap(self.Item2), self.Item3, self.Item4);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, R3, D) MapThird<A, B, C, D, R3>(this (A, B, C, D) self, Func<C, R3> thirdMap) =>
        (self.Item1, self.Item2, thirdMap(self.Item3), self.Item4);

    /// <summary>
    /// Fourth item-map to tuple
    /// </summary>
    [Pure]
    public static (A, B, C, R4) MapFourth<A, B, C, D, R4>(this(A, B, C, D) self, Func<D, R4> fourthMap) =>
        (self.Item1, self.Item2, self.Item3, fourthMap(self.Item4));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static (W, X, Y, Z) Select<A, B, C, D, W, X, Y, Z>(this (A, B, C, D) self, Func<(A, B, C, D), (W, X, Y, Z)> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D>(this (A, B, C, D) self, Action<A, B, C, D> func)
    {
        func(self.Item1, self.Item2, self.Item3, self.Item4);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A, B, C, D>(this (A, B, C, D) self, Action<A> first, Action<B> second, Action<C> third, Action<D> fourth)
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
    public static S Fold<A, B, C, D, S>(this (A, B, C, D) self, S state, Func<S, A, B, C, D, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3, self.Item4);

    /// <summary>
    /// Quad-fold
    /// </summary>
    [Pure]
    public static S QuadFold<A, B, C, D, S>(this(A, B, C, D) self, S state, Func<S, A, S> firstFold, Func<S, B, S> secondFold, Func<S, C, S> thirdFold, Func<S, D, S> fourthFold) =>
        fourthFold(thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3), self.Item4);

    /// <summary>
    /// Quad-fold back
    /// </summary>
    [Pure]
    public static S QuadFoldBack<A, B, C, D, S>(this (A, B, C, D) self, S state, Func<S, D, S> firstFold, Func<S, C, S> secondFold, Func<S, B, S> thirdFold, Func<S, A, S> fourthFold) =>
        fourthFold(thirdFold(secondFold(firstFold(state, self.Item4), self.Item3), self.Item2), self.Item1);
}
