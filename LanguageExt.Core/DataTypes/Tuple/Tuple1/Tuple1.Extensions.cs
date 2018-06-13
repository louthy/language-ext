using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

public static class Tuple1Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static Tuple<A, B> Add<A, B>(this Tuple<A> self, B second) =>
        Tuple(self.Item1, second);

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static Tuple<A> Append<SemiA, A>(this Tuple<A> a, Tuple<A> b)
        where SemiA : struct, Semigroup<A> =>
        Tuple(default(SemiA).Append(a.Item1, b.Item1));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static Tuple<A> Concat<MonoidA, A>(this Tuple<A> a, Tuple<A> b)
        where MonoidA : struct, Monoid<A> =>
        Tuple(mconcat<MonoidA, A>(a.Item1, b.Item1));

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static A Head<A>(this Tuple<A> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static A Last<A>(this Tuple<A> self) =>
        self.Item1;

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this Tuple<A> self)
        where NUM : struct, Num<A> =>
        self.Item1;

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this Tuple<A> self)
        where NUM : struct, Num<A> =>
        self.Item1;

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this Tuple<A> self, A value)
        where EQ : struct, Eq<A> =>
        default(EQ).Equals(self.Item1, value);

    /// <summary>
    /// Map to R
    /// </summary>
    [Pure]
    public static Tuple<R> Map<A, R>(this Tuple<A> self, Func<A, R> map) =>
        Tuple(map(self.Item1));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static Tuple<R> Select<A, R>(this Tuple<A> self, Func<A, R> map) =>
        Tuple(map(self.Item1));

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<A>(this Tuple<A> self, Action<A> func)
    {
        func(self.Item1);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S Fold<A, S>(this Tuple<A> self, S state, Func<S, A, S> fold) =>
        fold(state, self.Item1);
}
