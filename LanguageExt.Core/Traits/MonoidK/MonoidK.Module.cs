using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// A monoid on higher-kinds
/// </summary>
public static partial class MonoidK
{
    /// <summary>
    /// Identity
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, A> empty<F, A>()
        where F : MonoidK<F> =>
        F.Empty<A>();

    /// <summary>
    /// Associative binary operator
    /// </summary>
    [Pure]
    public static K<F, A> combine<F, A>(K<F, A> ma, K<F, A> mb)
        where F : MonoidK<F> =>
        F.Combine(ma, mb);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(K<M, A> mx, K<M, A> my, K<M, A> mz, params K<M, A>[] xs)
        where M : MonoidK<M> =>
        xs.AsIterable().Fold(combine, combine(combine(mx, my), mz));

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(IEnumerable<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.AsIterable().Fold(combine, M.Empty<A>());

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(Seq<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.Fold(combine, M.Empty<A>());
    
    /// <summary>
    /// Results in Empty if the predicate results in `false` 
    /// </summary>
    public static K<M, A> filter<M, A>(K<M, A> ma, Func<A, bool> predicate)
        where M : MonoidK<M>, Monad<M> =>
        M.Bind(ma, a => predicate(a) ? M.Pure(a) : M.Empty<A>());

    /// <summary>
    /// Chooses whether an element of the structure should be propagated through and if so
    /// maps the resulting value at the same time. 
    /// </summary>
    public static K<M, B> choose<M, A, B>(K<M, A> ma, Func<A, Option<B>> selector)
        where M : MonoidK<M>, Monad<M> =>
        M.Bind(ma, a => selector(a).Match(Some: M.Pure, None: M.Empty<B>));
    
    /// <summary>
    /// Conditional failure of `Alternative` computations. Defined by
    ///
    ///     guard(true)  = Applicative.pure
    ///     guard(false) = Alternative.empty
    ///
    /// </summary>
    /// <param name="flag"></param>
    /// <typeparam name="F"></typeparam>
    /// <returns></returns>
    [Pure]
    public static K<F, Unit> guard<F>(bool flag)
        where F : MonoidK<F>, Applicative<F> =>
        flag ? Applicative.pure<F, Unit>(default) : empty<F, Unit>();
}


