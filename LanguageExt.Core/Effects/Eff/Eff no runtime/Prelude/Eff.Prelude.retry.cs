using System;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retry<A>(
        Eff<A> ma) =>
        new(ma.Retry());

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retry<A>(
        Eff<A> ma, 
        Schedule schedule) =>
        new(ma.Retry(schedule));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retryWhile<A>(
        Eff<A> ma, 
        Func<Error, bool> predicate) => 
        new(ma.RetryWhile(predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retryWhile<A>(
        Eff<A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        new(ma.RetryWhile(schedule, predicate));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retryUntil<A>(
        Eff<A> ma,
        Func<Error, bool> predicate) =>
        new(ma.RetryUntil(predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static Eff<A> retryUntil<A>(
        Eff<A> ma,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        new(ma.RetryUntil(schedule, predicate));
}
