using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class Monoid
{
    /// <summary>
    /// The identity of combine
    /// </summary>
    [Pure]
    public static A empty<A>() where A : Monoid<A> =>
        A.Empty;

    /// <summary>
    /// Combine two structures
    /// </summary>
    [Pure]
    public static A combine<A>(A x, A y)
        where A : Monoid<A> =>
        x.Combine(y);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A combine<A>(A mx, A my, A mz, params A[] xs)
        where A : Monoid<A> =>
        Iterator.forward(xs)
                .Fold(combine, combine(combine(mx, my), mz));

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A combine<A>(Iterable<A> xs)
        where A : Monoid<A> =>
        xs.Fold(combine<A>, A.Empty);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static A combine<A>(Seq<A> xs)
        where A : Monoid<A> =>
        xs.Fold(combine, A.Empty);

    /// <summary>
    /// Get a concrete monoid instance value from a monoid supporting trait-type
    /// </summary>
    /// <typeparam name="A">Monoid type</typeparam>
    /// <returns>Monoid instance that can be passed around as a value</returns>
    [Pure]
    public static MonoidInstance<A> instance<A>()
        where A : Monoid<A> =>
        A.Instance;
}
