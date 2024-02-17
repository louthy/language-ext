/*
using System;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Keeps retrying the computation   
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> Retry<E, A>(
        this IO<E, A> ma) =>
        new(Transducer.retry(Schedule.Forever, ma.Morphism));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> Retry<E, A>(
        this IO<E, A> ma, 
        Schedule schedule) =>
        new(Transducer.retry(schedule, ma.Morphism));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> RetryWhile<E, A>(
        this IO<E, A> ma, 
        Func<E, bool> predicate) =>
        new(Transducer.retryWhile(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> RetryWhile<E, A>(
        this IO<E, A> ma,
        Schedule schedule,
        Func<E, bool> predicate) =>
        new(Transducer.retryWhile(schedule, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> RetryUntil<E, A>(
        this IO<E, A> ma,
        Func<E, bool> predicate) =>
        new(Transducer.retryUntil(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<E, A> RetryUntil<E, A>(
        this IO<E, A> ma,
        Schedule schedule,
        Func<E, bool> predicate) =>
        new(Transducer.retryUntil(schedule, ma.Morphism, predicate));
}
*/
