using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class MaybeMonadIOExtensions
{
    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, IO<A>> ToIO<M, A>(this K<M, A> ma)
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.ToIO(ma);

    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);

    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    public static K<M, ForkIO<A>> ForkIO<M, A>(this K<M, A> ma, Option<TimeSpan> timeout = default) 
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.ForkIO(ma, timeout);
}
