using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

public static class ValueTuple1Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static (A, B) Add<A, B>(this ValueTuple<A> self, B second) =>
        (self.Item1, second);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static ValueTuple<A> Append<SemiA, SemiB, A, B>(this ValueTuple<A> a, ValueTuple<A> b)
        where SemiA : struct, Semigroup<A> =>
        VTuple(default(SemiA).Append(a.Item1, b.Item1));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static ValueTuple<A> Concat<MonoidA, MonoidB, A, B>(this ValueTuple<A> a, ValueTuple<A> b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B> =>
        VTuple(mconcat<MonoidA, A>(a.Item1, b.Item1));

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A>(this ValueTuple<A> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static A Last<A>(this ValueTuple<A> self) =>
        self.Item1;

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this ValueTuple<A> self)
        where NUM : struct, Num<A> =>
        self.Item1;

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this ValueTuple<A> self)
        where NUM : struct, Num<A> =>
        self.Item1;

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this ValueTuple<A> self, A value)
        where EQ : struct, Eq<A> =>
        default(EQ).Equals(self.Item1, value);

    /// <summary>
    /// Map to R
    /// </summary>
    [Pure]
    public static ValueTuple<R> Map<A, R>(this ValueTuple<A> self, Func<A, R> map) =>
        VTuple(map(self.Item1));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R> Select<A, R>(this ValueTuple<A> self, Func<A, R> map) =>
        VTuple(map(self.Item1));

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A>(this ValueTuple<A> self, Action<A> func)
    {
        func(self.Item1);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S Fold<A, S>(this ValueTuple<A> self, S state, Func<S, A, S> fold) =>
        fold(state, self.Item1);
}