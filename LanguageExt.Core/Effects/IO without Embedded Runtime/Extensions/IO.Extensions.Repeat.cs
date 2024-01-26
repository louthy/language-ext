using System;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> Repeat<E, A>(this IO<E, A> ma) =>
        new(Transducer.repeat(Schedule.Forever, ma.Morphism));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> Repeat<E, A>(this IO<E, A> ma, Schedule schedule) =>
        new(Transducer.repeat(schedule, ma.Morphism));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> RepeatWhile<E, A>(this IO<E, A> ma, Func<A, bool> predicate) => 
        new(Transducer.repeatWhile(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> RepeatWhile<E, A>(
        this IO<E, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        new(Transducer.repeatWhile(schedule, ma.Morphism, predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> RepeatUntil<E, A>(
        this IO<E, A> ma,
        Func<A, bool> predicate) =>
        new(Transducer.repeatUntil(Schedule.Forever, ma.Morphism, predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static IO<E, A> RepeatUntil<E, A>(
        this IO<E, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        new(Transducer.repeatUntil(schedule, ma.Morphism, predicate));
}
