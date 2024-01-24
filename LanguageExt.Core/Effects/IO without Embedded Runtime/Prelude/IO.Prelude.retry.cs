/*
#nullable enable
using System;
using LanguageExt.Effects.Traits;

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
    public static IO<RT, E, A> retry<RT, E, A>(IO<RT, E, A> ma)
        where RT : HasIO<RT, E> =>
        new(Transducer.retry(Schedule.Forever, ma.Morphism));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<RT, E, A> retry<RT, E, A>(Schedule schedule, IO<RT, E, A> ma)
        where RT : HasIO<RT, E> =>
        new(Transducer.retry(schedule, ma.Morphism));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<RT, E, A> retryWhile<RT, E, A>(
        IO<RT, E, A> ma,
        Func<E, bool> predicate) where RT : HasIO<RT, E> =>
        new(Transducer.retryWhile(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<RT, E, A> retryWhile<RT, E, A>(
        Schedule schedule,
        IO<RT, E, A> ma,
        Func<E, bool> predicate)
        where RT : HasIO<RT, E> =>
        new(Transducer.retryWhile(schedule, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<RT, E, A> retryUntil<RT, E, A>(
        IO<RT, E, A> ma,
        Func<E, bool> predicate)
        where RT : HasIO<RT, E> =>
        new(Transducer.retryUntil(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for retrying</param>
    /// <param name="ma">Computation to retry</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of ma</returns>
    public static IO<RT, E, A> retryUntil<RT, E, A>(
        Schedule schedule,
        IO<RT, E, A> ma,
        Func<E, bool> predicate)
        where RT : HasIO<RT, E> =>
        new(Transducer.retryUntil(schedule, ma.Morphism, predicate));
}
*/
