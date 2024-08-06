using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class MonadIO
{
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        M.LiftIO(ma);
    
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(K<IO, A> ma) 
        where M : Monad<M> =>
        M.LiftIO(ma);

    /// <summary>
    /// Get the `IO` monad from within the `M` monad
    /// </summary>
    /// <remarks>
    /// This only works if the `M` trait implements `MonadIO.ToIO`.
    /// </remarks>
    public static K<M, IO<A>> toIO<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        M.ToIO(ma);
    
    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> mapIO<M, A, B>(Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);
}
