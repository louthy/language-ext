using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class MaybeMonadUnliftIOExtensions
{
    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, IO<A>> ToIO<M, A>(this K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.ToIO(ma);

    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : MonadUnliftIO<M> =>
        M.MapIO(ma, f);

    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> MapIO<M, A, B>(this Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.MapIO(ma, f);
    
    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, IO<A>> ToIOMaybe<M, A>(this K<M, A> ma)
        where M : MonadIO<M> =>
        M.ToIO(ma);

    /// <summary>
    /// Convert an action in `ma`to an action in `IO`.
    /// </summary>
    public static K<M, B> MapIOMaybe<M, A, B>(this K<M, A> ma, Func<IO<A>, IO<B>> f)
        where M : MonadIO<M> =>
        M.MapIO(ma, f);

    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> MapIOMaybe<M, A, B>(this Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadIO<M> =>
        M.MapIO(ma, f);    
}
