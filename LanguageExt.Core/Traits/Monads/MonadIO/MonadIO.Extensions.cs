using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Monad that is either the IO monad or a transformer with the IO monad in its stack
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
public static class MonadIOExtensions
{
    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, IO<A>> ToIO<M, A>(this K<M, A> ma)
        where M : MonadIO<M>, Monad<M> =>
        M.ToIO(ma);

    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);

    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);
}
