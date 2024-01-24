#nullable enable
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class TypeClass
{
    /// <summary>
    /// The identity of append
    /// <summary>
    [Pure]
    public static A mempty<MONOID, A>() where MONOID : Monoid<A> =>
        MONOID.Empty();

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A mconcat<MONOID, A>(IEnumerable<A> xs) where MONOID : Monoid<A> =>
        xs.Fold(MONOID.Empty(), MONOID.Append);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A mconcat<MONOID, A>(params A[] xs) where MONOID : Monoid<A> =>
        xs.Fold(MONOID.Empty(), MONOID.Append);
}
