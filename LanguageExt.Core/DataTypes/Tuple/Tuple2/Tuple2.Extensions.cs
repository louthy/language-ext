using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

public static class Tuple2Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B, C> add<A, B, C>(this Tuple<A, B> self, C third) =>
        Tuple(self.Item1, self.Item2, third);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static Tuple<A, B> Append<SemiA, SemiB, A, B>(this Tuple<A, B> a, Tuple<A, B> b)
        where SemiA : struct, Semigroup<A>
        where SemiB : struct, Semigroup<B> =>
        Tuple(default(SemiA).Append(a.Item1, b.Item1),
              default(SemiB).Append(a.Item2, b.Item2));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this Tuple<A, A> a)
        where SemiA : struct, Semigroup<A> =>
        default(SemiA).Append(a.Item1, a.Item2);

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static Tuple<A, B> Concat<MonoidA, MonoidB, A, B>(this Tuple<A, B> a, Tuple<A, B> b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B> =>
        Tuple(mconcat<MonoidA, A>(a.Item1, b.Item1),
              mconcat<MonoidB, B>(a.Item2, b.Item2));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A Concat<MonoidA, A>(this Tuple<A, A> a)
        where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static T1 Head<T1, T2>(this Tuple<T1, T2> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static T2 Last<T1, T2>(this Tuple<T1, T2> self) =>
        self.Item2;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static Tuple<T2> Tail<T1, T2>(this Tuple<T1, T2> self) =>
        Tuple(self.Item2);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this Tuple<A, A> self)
        where NUM : struct, Num<A> =>
        TypeClass.sum<NUM, FoldTuple<A>, Tuple<A, A>, A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this Tuple<A, A> self)
        where NUM : struct, Num<A> =>
        TypeClass.product<NUM, FoldTuple<A>, Tuple<A, A>, A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this Tuple<A, A> self, A value)
        where EQ : struct, Eq<A> =>
        TypeClass.contains<EQ, FoldTuple<A>, Tuple<A, A>, A>(self, value);

    /// <summary>
    /// Map 
    /// </summary>
    [Pure]
    public static R Map<T1, T2, R>(this Tuple<T1, T2> self, Func<T1, T2, R> map) =>
        map(self.Item1, self.Item2);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, R>(this Tuple<A, B> self, Func<Tuple<A, B>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static Tuple<Y, Z> Map<A, B, Y, Z>(this Tuple<A, B> self, Func<A, B, Tuple<Y, Z>> map) =>
        map(self.Item1, self.Item2);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static Tuple<R1, R2> Map<T1, T2, R1, R2>(this Tuple<T1, T2> self, Func<Tuple<T1, T2>, Tuple<R1, R2>> map) =>
        map(self);

    /// <summary>
    /// Bi-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R1, R2> BiMap<T1, T2, R1, R2>(this Tuple<T1, T2> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap) =>
        Tuple(firstMap(self.Item1), secondMap(self.Item2));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R1, T2> MapFirst<T1, T2, R1>(this Tuple<T1, T2> self, Func<T1, R1> firstMap) =>
        Tuple(firstMap(self.Item1), self.Item2);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static Tuple<T1, R2> MapSecond<T1, T2, R2>(this Tuple<T1, T2> self, Func<T2, R2> secondMap) =>
        Tuple(self.Item1, secondMap(self.Item2));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R1, R2> Select<T1, T2, R1, R2>(this Tuple<T1, T2> self, Func<Tuple<T1, T2>, Tuple<R1, R2>> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> func)
    {
        func(self.Item1, self.Item2);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2>(this Tuple<T1, T2> self, Action<T1> first, Action<T2> second)
    {
        first(self.Item1);
        second(self.Item2);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S Fold<T1, T2, S>(this Tuple<T1, T2> self, S state, Func<S, T1, T2, S> fold) =>
        fold(state, self.Item1, self.Item2);

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFold<T1, T2, S>(this Tuple<T1, T2> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold) =>
        secondFold(firstFold(state, self.Item1), self.Item2);

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFoldBack<T1, T2, S>(this Tuple<T1, T2> self, S state, Func<S, T2, S> firstFold, Func<S, T1, S> secondFold) =>
        secondFold(firstFold(state, self.Item2), self.Item1);
}