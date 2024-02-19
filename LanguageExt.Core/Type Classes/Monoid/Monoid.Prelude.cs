using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Trait
{
    /// <summary>
    /// The identity of append
    /// </summary>
    [Pure]
    public static A mempty<A>() where A : Monoid<A> =>
        A.Empty;

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A mconcat<A>(IEnumerable<A> xs) where A : Monoid<A> =>
        xs.Fold(A.Empty, (x, y) => x.Append(y));

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A mconcat<A>(params A[] xs) where A : Monoid<A> =>
        xs.Fold(A.Empty, (x, y) => x.Append(y));
}
