using System;
using System.Diagnostics.Contracts;
using System.Threading;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class MonadIO
{
    /// <summary>
    /// When the predicate evaluates to `true`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> when<M>(K<M, bool> Pred, K<IO, Unit> Then)
        where M : MonadIO<M> =>
        Pred.Bind(f => Applicative.when(f, Then).As());

    /// <summary>
    /// When the predicate evaluates to `false`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> unless<M>(K<M, bool> Pred, K<IO, Unit> Then)
        where M : MonadIO<M> =>
        Pred.Bind(f => Applicative.unless(f, Then).As());
    
    /// <summary>
    /// Embeds the `IO` monad into the `M〈A〉` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    [Pure]
    public static K<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.LiftIO(ma);
    
    /// <summary>
    /// Embeds the `IO` monad into the `M〈A〉` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    [Pure]
    public static K<M, A> liftIO<M, A>(K<IO, A> ma) 
        where M : Maybe.MonadIO<M>, Monad<M> =>
        M.LiftIO(ma);

    /// <summary>
    /// Get the environment value threaded through the IO computation
    /// </summary>
    /// <typeparam name="M">Trait</typeparam>
    /// <returns>Lifted environment value</returns>
    [Pure]
    public static K<M, EnvIO> envIO<M>()  
        where M : MonadIO<M>, Monad<M> =>
        M.EnvIO;
    
    /// <summary>
    /// Get the cancellation token threaded through the IO computation
    /// </summary>
    /// <typeparam name="M">Trait</typeparam>
    /// <returns>Lifted cancellation token</returns>
    [Pure]
    public static K<M, CancellationToken> token<M>()  
        where M : MonadIO<M>, Monad<M> =>
        M.Token;

    /// <summary>
    /// Get the cancellation token-source threaded through the IO computation
    /// </summary>
    /// <typeparam name="M">Trait</typeparam>
    /// <returns>Lifted cancellation token-source</returns>
    [Pure]
    public static K<M, CancellationTokenSource> tokenSource<M>() 
        where M : MonadIO<M>, Monad<M> =>
        M.TokenSource;

    /// <summary>
    /// Get the synchronisation-context threaded through the IO computation
    /// </summary>
    /// <typeparam name="M">Trait</typeparam>
    /// <returns>Lifted synchronisation-context</returns>
    [Pure]
    public static K<M, Option<SynchronizationContext>> syncContext<M>() 
        where M : MonadIO<M>, Monad<M> =>
        M.SyncContext;
}
