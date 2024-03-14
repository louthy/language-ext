using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static partial class AlternativeExtensions
{
    /// <summary>
    /// Results in Empty if the predicate results in `false` 
    /// </summary>
    public static K<M, A> Filter<M, A>(this K<M, A> ma, Func<A, bool> predicate)
        where M : Alternative<M>, Monad<M> =>
        M.Bind(ma, a => predicate(a) ? M.Pure(a) : M.Empty<A>());
    
    /// <summary>
    /// Results in Empty if the predicate results in `false` 
    /// </summary>
    public static K<M, A> Where<M, A>(this K<M, A> ma, Func<A, bool> predicate)
        where M : Alternative<M>, Monad<M> =>
        M.Bind(ma, a => predicate(a) ? M.Pure(a) : M.Empty<A>());

    /// <summary>
    /// Chooses whether an element of the structure should be propagated through and if so
    /// maps the resulting value at the same time. 
    /// </summary>
    public static K<M, B> Choose<M, A, B>(this K<M, A> ma, Func<A, Option<B>> selector)
        where M : Alternative<M>, Monad<M> =>
        M.Bind(ma, a => selector(a).Match(Some: M.Pure, None: M.Empty<B>));
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> OneOf<F, A>(this Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        Alternative.oneOf(ms);

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
    public static K<F, Seq<A>> Some<F, A>(this K<F, A> v)
        where F : Alternative<F> =>
        F.Some(v);

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
    public static K<F, Seq<A>> Many<F, A>(this K<F, A> v)
        where F : Alternative<F> =>
        F.Many(v);
}
