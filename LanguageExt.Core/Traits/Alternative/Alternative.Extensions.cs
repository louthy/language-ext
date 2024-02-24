using System;

namespace LanguageExt.Traits;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static partial class Alternative
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
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> OneOf<F, A>(this Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        oneOf(ms);

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
