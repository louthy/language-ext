using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.DSL;

namespace LanguageExt.Traits;

/// <summary>
/// Monad that is either the IO monad or a transformer with the IO monad in its stack
/// </summary>
/// <typeparam name="M">Self-referring trait</typeparam>
public interface MonadIO<M> : Monad<M>
    where M : MonadIO<M>
{
    /// <summary>
    /// Lifts the IO monad into a monad transformer stack.  
    /// </summary>
    /// <param name="ma">IO computation to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    public static virtual K<M, A> LiftIO<A>(K<IO, A> ma) =>
        M.LiftIO(ma.As());

    /// <summary>
    /// Lifts the IO monad into a monad transformer stack.  
    /// </summary>
    /// <remarks>
    /// IMPLEMENTATION REQUIRED: If this method isn't overloaded in this monad
    /// or any monad in the stack on the way to the inner-monad, then it will throw
    /// an exception.
    ///
    /// This isn't ideal, it appears to be the only way to achieve this
    /// kind of functionality in C# without resorting to magic. 
    /// </remarks>
    /// <param name="ma">IO computation to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    /// <exception cref="ExceptionalException">If this method isn't overloaded in
    /// the inner monad or any monad in the stack on the way to the inner-monad,
    /// then it will throw an exception.</exception>
    public static abstract K<M, A> LiftIO<A>(IO<A> ma);

    static K<M, A> Maybe.MonadIO<M>.LiftIOMaybe<A>(IO<A> ma) => 
        M.LiftIO(ma);

    static K<M, A> Maybe.MonadIO<M>.LiftIOMaybe<A>(K<IO, A> ma) => 
        M.LiftIO(ma);
    
    public static virtual K<M, EnvIO> EnvIO => 
        M.LiftIOMaybe(IO.env);
    
    public static virtual K<M, CancellationToken> Token => 
        M.LiftIOMaybe(IO.token);

    public static virtual K<M, CancellationTokenSource> TokenSource =>
        M.LiftIOMaybe(IO.source);

    public static virtual K<M, Option<SynchronizationContext>> SyncContext =>
        M.LiftIOMaybe(IO.syncContext);
    
}
