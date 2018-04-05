using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

public static class ValueTuple3Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, T2, T3, T4> Add<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3> self, T4 fourth) =>
        (self.Item1, self.Item2, self.Item3, fourth);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static ValueTuple<A, B, C> Append<SemiA, SemiB, SemiC, A, B, C>(this ValueTuple<A, B, C> a, ValueTuple<A, B, C> b)
        where SemiA : struct, Semigroup<A>
        where SemiB : struct, Semigroup<B>
        where SemiC : struct, Semigroup<C> =>
        (default(SemiA).Append(a.Item1, b.Item1),
         default(SemiB).Append(a.Item2, b.Item2),
         default(SemiC).Append(a.Item3, b.Item3));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this ValueTuple<A, A, A> a)
        where SemiA : struct, Semigroup<A> =>
        default(SemiA).Append(a.Item1,
            default(SemiA).Append(a.Item2, a.Item3));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static ValueTuple<A, B, C> Concat<MonoidA, MonoidB, MonoidC, A, B, C>(this ValueTuple<A, B, C> a, ValueTuple<A, B, C> b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B>
        where MonoidC : struct, Monoid<C> =>
        (mconcat<MonoidA, A>(a.Item1, b.Item1),
         mconcat<MonoidB, B>(a.Item2, b.Item2),
         mconcat<MonoidC, C>(a.Item3, b.Item3));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A Concat<MonoidA, A>(this ValueTuple<A, A, A> a)
        where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2, a.Item3);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static T1 Head<T1, T2, T3>(this ValueTuple<T1, T2, T3> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static T3 Last<T1, T2, T3>(this ValueTuple<T1, T2, T3> self) =>
        self.Item3;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T2, T3> Tail<T1, T2, T3>(this ValueTuple<T1, T2, T3> self) =>
        (self.Item2, self.Item3);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this ValueTuple<A, A, A> self)
        where NUM : struct, Num<A> =>
        default(NUM).Plus(self.Item1, default(NUM).Plus(self.Item2, self.Item3));

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this ValueTuple<A, A, A> self)
        where NUM : struct, Num<A> =>
        default(NUM).Product(self.Item1, default(NUM).Product(self.Item2, self.Item3));

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this ValueTuple<A, A, A> self, A value)
        where EQ : struct, Eq<A> =>
        default(EQ).Equals(self.Item1, value) ||
        default(EQ).Equals(self.Item2, value) ||
        default(EQ).Equals(self.Item3, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, R>(this ValueTuple<A, B, C> self, Func<ValueTuple<A, B, C>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, C, R>(this ValueTuple<A, B, C> self, Func<A, B, C, R> map) =>
        map(self.Item1, self.Item2, self.Item3);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, R2, R3> Map<T1, T2, T3, R1, R2, R3>(this ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap, Func<T3, R3> thirdMap) =>
        (firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, T2, T3> MapFirst<T1, T2, T3, R1>(this ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap) =>
        (firstMap(self.Item1), self.Item2, self.Item3);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, R2, T3> MapSecond<T1, T2, T3, R2>(this ValueTuple<T1, T2, T3> self, Func<T2, R2> secondMap) =>
        (self.Item1, secondMap(self.Item2), self.Item3);

    /// <summary>
    /// Third item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, T2, R3> MapThird<T1, T2, T3, R3>(this ValueTuple<T1, T2, T3> self, Func<T3, R3> thirdMap) =>
        (self.Item1, self.Item2, thirdMap(self.Item3));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, R2, R3> Select<T1, T2, T3, R1, R2, R3>(this ValueTuple<T1, T2, T3> self, Func<ValueTuple<T1, T2, T3>, ValueTuple<R1, R2, R3>> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3>(this ValueTuple<T1, T2, T3> self, Action<T1, T2, T3> func)
    {
        func(self.Item1, self.Item2, self.Item3);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2, T3>(this ValueTuple<T1, T2, T3> self, Action<T1> first, Action<T2> second, Action<T3> third)
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
    public static S Fold<T1, T2, T3, S>(this ValueTuple<T1, T2, T3> self, S state, Func<S, T1, T2, T3, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3);

    /// <summary>
    /// Tri-fold
    /// </summary>
    [Pure]
    public static S TriFold<T1, T2, T3, S>(this ValueTuple<T1, T2, T3> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold, Func<S, T3, S> thirdFold) =>
        thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3);

    /// <summary>
    /// Tri-fold
    /// </summary>
    [Pure]
    public static S TriFoldBack<T1, T2, T3, S>(this ValueTuple<T1, T2, T3> self, S state, Func<S, T3, S> firstFold, Func<S, T2, S> secondFold, Func<S, T1, S> thirdFold) =>
        thirdFold(secondFold(firstFold(state, self.Item3), self.Item2), self.Item1);
}