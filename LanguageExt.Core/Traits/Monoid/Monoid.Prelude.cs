using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class Monoid
{
    /// <summary>
    /// The identity of append
    /// </summary>
    [Pure]
    public static A empty<A>() where A : Monoid<A> =>
        A.Empty;

    /// <summary>
    /// The identity of append
    /// </summary>
    [Pure]
    public static A append<A>(A x, A y) where A : Monoid<A> =>
        x + y;

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A concat<A>(IEnumerable<A> xs) where A : Monoid<A> =>
        xs.Fold(A.Empty, (x, y) => x.Append(y));

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A concat<A>(params A[] xs) where A : Monoid<A> =>
        xs.Fold(A.Empty, (x, y) => x.Append(y));
}
