using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, T2, T3, T4> add<T1, T2, T3, T4>(ValueTuple<T1, T2, T3> self, T4 fourth) =>
        (self.Item1, self.Item2, self.Item3, fourth);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static T1 head<T1, T2, T3>(ValueTuple<T1, T2, T3> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static T3 last<T1, T2, T3>(ValueTuple<T1, T2, T3> self) =>
        self.Item3;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T2, T3> tail<T1, T2, T3>(ValueTuple<T1, T2, T3> self) =>
        (self.Item2, self.Item3);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A sum<NUM, A>(ValueTuple<A, A, A> self)
        where NUM : Num<A> =>
        NUM.Plus(self.Item1, NUM.Plus(self.Item2, self.Item3));

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A product<NUM, A>(ValueTuple<A, A, A> self)
        where NUM : Num<A> =>
        NUM.Product(self.Item1, NUM.Product(self.Item2, self.Item3));

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool contains<EQ, A>(ValueTuple<A, A, A> self, A value)
        where EQ : Eq<A> =>
        EQ.Equals(self.Item1, value) ||
        EQ.Equals(self.Item2, value) ||
        EQ.Equals(self.Item3, value);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R map<A, B, C, R>(ValueTuple<A, B, C> self, Func<ValueTuple<A, B, C>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R map<A, B, C, R>(ValueTuple<A, B, C> self, Func<A, B, C, R> map) =>
        map(self.Item1, self.Item2, self.Item3);

    /// <summary>
    /// Tri-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, R2, R3> trimap<T1, T2, T3, R1, R2, R3>(ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap, Func<T3, R3> thirdMap) =>
        ValueTuple.Create(firstMap(self.Item1), secondMap(self.Item2), thirdMap(self.Item3));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, T2, T3> mapFirst<T1, T2, T3, R1>(ValueTuple<T1, T2, T3> self, Func<T1, R1> firstMap) =>
        ValueTuple.Create(firstMap(self.Item1), self.Item2, self.Item3);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, R2, T3> mapSecond<T1, T2, T3, R2>(ValueTuple<T1, T2, T3> self, Func<T2, R2> secondMap) =>
        ValueTuple.Create(self.Item1, secondMap(self.Item2), self.Item3);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, T2, R3> mapThird<T1, T2, T3, R3>(ValueTuple<T1, T2, T3> self, Func<T3, R3> thirdMap) =>
        ValueTuple.Create(self.Item1, self.Item2, thirdMap(self.Item3));

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit iter<T1, T2, T3>(ValueTuple<T1, T2, T3> self, Action<T1, T2, T3> func)
    {
        func(self.Item1, self.Item2, self.Item3);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit iter<T1, T2, T3>(ValueTuple<T1, T2, T3> self, Action<T1> first, Action<T2> second, Action<T3> third)
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
    public static S fold<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T1, T2, T3, S> fold) =>
        fold(state, self.Item1, self.Item2, self.Item3);

    /// <summary>
    /// Tri-fold
    /// </summary>
    [Pure]
    public static S trifold<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold, Func<S, T3, S> thirdFold) =>
        thirdFold(secondFold(firstFold(state, self.Item1), self.Item2), self.Item3);

    /// <summary>
    /// Tri-fold
    /// </summary>
    [Pure]
    public static S trifoldBack<T1, T2, T3, S>(ValueTuple<T1, T2, T3> self, S state, Func<S, T3, S> firstFold, Func<S, T2, S> secondFold, Func<S, T1, S> thirdFold) =>
        thirdFold(secondFold(firstFold(state, self.Item3), self.Item2), self.Item1);
}
