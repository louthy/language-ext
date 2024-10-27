using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static class MonoidKExtensions
{
    /// <summary>
    /// Results in Empty if the predicate results in `false` 
    /// </summary>
    public static K<M, A> Filter<M, A>(this K<M, A> ma, Func<A, bool> predicate)
        where M : MonoidK<M>, Monad<M> =>
        M.Bind(ma, a => predicate(a) ? M.Pure(a) : M.Empty<A>());
    
    /// <summary>
    /// Results in Empty if the predicate results in `false` 
    /// </summary>
    public static K<M, A> Where<M, A>(this K<M, A> ma, Func<A, bool> predicate)
        where M : MonoidK<M>, Monad<M> =>
        M.Bind(ma, a => predicate(a) ? M.Pure(a) : M.Empty<A>());

    /// <summary>
    /// Chooses whether an element of the structure should be propagated through and if so
    /// maps the resulting value at the same time. 
    /// </summary>
    public static K<M, B> Choose<M, A, B>(this K<M, A> ma, Func<A, Option<B>> selector)
        where M : MonoidK<M>, Monad<M> =>
        M.Bind(ma, a => selector(a).Match(Some: M.Pure, None: M.Empty<B>));
}
