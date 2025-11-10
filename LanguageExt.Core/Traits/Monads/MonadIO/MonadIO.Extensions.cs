using System;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

public static class MonadIOExtensions
{
    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static K<M, B> Bind<M, A, B>(
        this K<M, A> ma,
        Func<A, IO<B>> f)
        where M : MonadIO<M>, Monad<M> =>
        M.Bind(ma, x => M.LiftIOMaybe(f(x)));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static K<M, B> Bind<M, A, B>(
        this IO<A> ma,
        Func<A, K<M, B>> f)
        where M : MonadIO<M>, Monad<M> =>
        M.Bind(M.LiftIOMaybe(ma), f);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static K<M, C> SelectMany<M, A, B, C>(this K<M, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M> =>
        ma.Bind(x => IOTail<A>.resolve(x, bind(x), project));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    public static K<M, C> SelectMany<M, A, B, C>(this IO<A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M>, Monad<M> =>
        M.SelectMany(M.LiftIOMaybe(ma), bind, project);
    
    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    public static K<M, ForkIO<A>> ForkIO<M, A>(this K<M, A> ma, Option<TimeSpan> timeout = default) 
        where M : MonadUnliftIO<M> =>
        M.ForkIO(ma, timeout);
    
    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    public static K<M, ForkIO<A>> ForkIOMaybe<M, A>(this K<M, A> ma, Option<TimeSpan> timeout = default) 
        where M : MonadIO<M> =>
        M.ForkIOMaybe(ma, timeout);    
}
