using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Trait;
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
    public static ValueTuple<A> Append<SemiA, A>(this ValueTuple<A> a, ValueTuple<A> b)
        where SemiA : Semigroup<A> =>
        VTuple(SemiA.Append(a.Item1, b.Item1));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static ValueTuple<A> Concat<MonoidA, A>(this ValueTuple<A> a, ValueTuple<A> b)
        where MonoidA : Monoid<A> =>
        VTuple(mconcat<MonoidA, A>(a.Item1, b.Item1));

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this ValueTuple<A> self, A value)
        where EQ : Eq<A> =>
        EQ.Equals(self.Item1, value);

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
