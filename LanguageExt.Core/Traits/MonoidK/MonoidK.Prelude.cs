using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static class MonoidK
{
    /// <summary>
    /// The identity of append
    /// </summary>
    [Pure]
    public static K<M, A> empty<M, A>() where M : MonoidK<M> =>
        M.Empty<A>();

    /// <summary>
    /// Combine two structures
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(K<M, A> x, K<M, A> y)
        where M : MonoidK<M> =>
        x.Combine(y);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(K<M, A> mx, K<M, A> my, K<M, A> mz, params K<M, A>[] xs)
        where M : MonoidK<M> =>
        xs.AsEnumerableM().Fold(combine(combine(mx, my), mz), combine);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(IEnumerable<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.AsEnumerableM().Fold(M.Empty<A>(), combine);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(Seq<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.Fold(M.Empty<A>(), combine);
}
