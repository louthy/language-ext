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
        xs.AsIterable().Fold(combine(combine(mx, my), mz), combine);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(IEnumerable<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.AsIterable().Fold(M.Empty<A>(), combine);

    /// <summary>
    /// Fold a list using the monoid.
    /// </summary>
    [Pure]
    public static K<M, A> combine<M, A>(Seq<K<M, A>> xs)
        where M : MonoidK<M> =>
        xs.Fold(M.Empty<A>(), combine);
    
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
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(params K<F, A>[] ms)
        where F : MonoidK<F> =>
        oneOf(ms.AsIterable().ToSeq());

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(Seq<K<F, A>> ms)
        where F : MonoidK<F> =>
        ms.IsEmpty
            ? F.Empty<A>()
            : F.Combine(ms.Head.Value!, oneOf(ms.Tail));
    
    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static K<F, Seq<A>> some<F, A>(K<F, A> v)
        where F : MonoidK<F>, Applicative<F>
    {
        // TODO: Make lazy
        return some_v();
        
        K<F, Seq<A>> many_v() =>
            F.Combine(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v());
    }
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static K<F, Seq<A>> many<F, A>(K<F, A> v)
        where F : MonoidK<F>, Applicative<F>
    {
        // TODO: Make lazy
        return many_v();
        
        K<F, Seq<A>> many_v() =>
            F.Combine(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v());
    }    
    
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
    
    static class Append<M, A>
        where M : Applicative<M>
    {
        public static readonly K<M, Func<A, Seq<A>, Seq<A>>> cons =
            M.Pure((A x, Seq<A> y) => x.Cons(y));
    }    
}
