using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// The `@catch` functions produce a `CatchError` or `CatchValue` type. These can be composed together with the
/// `Aff` and `Eff` monads (and maybe more in the future), to create a functional-programming equivalent to
/// exception catching and matching.
/// </summary>
/// <example>
/// 
///      public class TimeoutExample<RT>
///          where RT : struct,
///          HasTime<RT>,
///          HasCancel<RT>,
///          HasConsole<RT>
///      {
///          public static Eff<RT, Unit> main =>
///              from _1 in timeout(60 * seconds, longRunning)
///                       | @catch(Errors.TimedOut, unitEff)
///              from _2 in Console<RT>.writeLine("done")
///              select unit;
///      
///          static Aff<RT, Unit> longRunning =>
///              (from tm in Time<RT>.now
///               from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
///               select unit)
///             .ToAff()
///             .Repeat(Schedule.Fibonacci(1 * second));
///      }
/// </example>
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
        matchError(e => e?.Equals(error) ?? false, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<E, M, A> @catch<E, M, A>(E error, K<M, A> Fail)
        where M : Fallible<E, M> =>
        matchError(e => e?.Equals(error) ?? false, (E _) => Fail);
    
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
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Error specific
    //
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(int errorCode, Func<Error, K<M, A>> Fail)
        where M : Fallible<Error, M> =>
        matchError(e => e.Code == errorCode, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(int errorCode, K<M, A> Fail)
        where M : Fallible<Error, M> =>
        matchError(e => e.Code == errorCode, (Error _) => Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(Func<Error, bool> predicate, Func<Error, K<M, A>> Fail)
        where M : Fallible<Error, M> =>
        matchError(predicate, Fail);
    
    /// <summary>
    /// Catch an error if the error matches the argument provided 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(Func<Error, bool> predicate, K<M, A> Fail) 
        where M : Fallible<Error, M> =>
        matchError(predicate, _ => Fail);
    
    /// <summary>
    /// Catch all errors and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(Func<Error, K<M, A>> Fail) 
        where M : Fallible<M> =>
        matchError(static _ => true, Fail);
    
    /// <summary>
    /// Catch all `Error` errors of subtype `E` and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> catchOf<E, M, A>(Func<E, K<M, A>> Fail) 
        where E : Error 
        where M : Fallible<Error, M> =>
        matchError<Error, M, A>(static e => e is E, e => Fail((E)e));
    
    /// <summary>
    /// Catch all errors and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> @catch<M, A>(K<M, A> Fail) 
        where M : Fallible<Error, M> =>
        matchError(static _ => true, (Error _) => Fail);
    
    
    /// <summary>
    /// Catch all `Expected` errors and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> expected<M, A>(Func<Expected, K<M, A>> Fail) 
        where M : Fallible<M> =>
        catchOf(Fail);
    
    /// <summary>
    /// Catch all `Expected` errors of subtype `E` and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> expectedOf<E, M, A>(Func<E, K<M, A>> Fail)
        where E : Expected 
        where M : Fallible<Error, M> =>
        catchOf(Fail);
    
    /// <summary>
    /// Catch all `Exceptional` errors and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> exceptional<M, A>(Func<Exceptional, K<M, A>> Fail) 
        where M : Fallible<Error, M> =>
        catchOf(Fail);
    
    /// <summary>
    /// Catch all errors and return Fail 
    /// </summary>
    public static CatchM<Error, M, A> exceptionalOf<E, M, A>(Func<E, K<M, A>> Fail) 
        where E : Exceptional 
        where M : Fallible<Error, M> =>
        catchOf(Fail);
}
