using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> Retry<RT, A>(
        this Eff<RT, A> ma)
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.Retry());

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> Retry<RT, A>(
        this Eff<RT, A> ma, 
        Schedule schedule)
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.Retry(schedule));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> RetryWhile<RT, A>(
        this Eff<RT, A> ma, 
        Func<Error, bool> predicate) 
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.RetryWhile(predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> RetryWhile<RT, A>(
        this Eff<RT, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate)
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.RetryWhile(schedule, predicate));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> RetryUntil<RT, A>(
        this Eff<RT, A> ma,
        Func<Error, bool> predicate)
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.RetryUntil(predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<RT, A> RetryUntil<RT, A>(
        this Eff<RT, A> ma,
        Schedule schedule,
        Func<Error, bool> predicate)
        where RT : HasIO<RT, Error> =>
        ma.MapIO(io => io.RetryUntil(schedule, predicate));
}
