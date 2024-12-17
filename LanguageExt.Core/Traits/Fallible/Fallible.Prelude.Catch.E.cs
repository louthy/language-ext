using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Catch an error if the predicate matches
    /// </summary>
    internal static CatchM<E, M, A> matchError<E, M, A>(Func<E, bool> predicate, Func<E, K<M, A>> Fail) 
        where M : Fallible<E, M> =>
        new (predicate, Fail);
    
    /// <summary>
    /// Catch an error if the predicate matches
    /// </summary>
    internal static CatchM<E, M, A> matchError<E, M, A>(Func<E, bool> predicate, Func<E, K<IO, A>> Fail) 
        where M : Fallible<E, M>, Monad<M> =>
        new (predicate, e => MonadIO.liftIO<M, A>(Fail(e)));
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(E error, Func<E, K<M, A>> Fail) 
        where M : Fallible<E, M> =>
        matchError(e => error?.Equals(e) ?? false, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(E error, K<M, A> Fail)
        where M : Fallible<E, M> =>
        matchError(e => error?.Equals(e) ?? false, (E _) => Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(Func<E, bool> predicate, Func<E, K<M, A>> Fail)
        where M : Fallible<E, M> =>
        matchError(predicate, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(Func<E, bool> predicate, K<M, A> Fail) 
        where M : Fallible<E, M> =>
        matchError(predicate, _ => Fail);
    
    /// <summary>
    /// Catch all errors and return Fail 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(Func<E, K<M, A>> Fail) 
        where M : Fallible<E, M> =>
        matchError(static _ => true, Fail);
}
