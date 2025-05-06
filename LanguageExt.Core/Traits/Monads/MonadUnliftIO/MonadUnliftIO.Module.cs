using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class MonadUnliftIO
{
    /// <summary>
    /// Get the `IO` monad from within the `M` monad
    /// </summary>
    /// <remarks>
    /// This only works if the `M` trait implements `MonadIO.ToIO`.
    /// </remarks>
    [Pure]
    public static K<M, IO<A>> toIO<M, A>(K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.ToIO(ma);
    
    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    [Pure]
    public static K<M, B> mapIO<M, A, B>(Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadUnliftIO<M> =>
        M.MapIO(ma, f);
}
