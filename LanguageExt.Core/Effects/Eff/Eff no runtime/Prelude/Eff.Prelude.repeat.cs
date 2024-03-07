using System;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeat<A>(Eff<A> ma) =>
        ma.Repeat();

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeat<A>(Schedule schedule, Eff<A> ma) =>
        ma.Repeat(schedule);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeatWhile<A>(Eff<A> ma, Func<A, bool> predicate) => 
        ma.RepeatWhile(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeatWhile<A>(
        Schedule schedule,
        Eff<A> ma,
        Func<A, bool> predicate) =>
        ma.RepeatWhile(schedule, predicate);

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeatUntil<A>(
        Eff<A> ma,
        Func<A, bool> predicate) =>
        ma.RepeatUntil(predicate);

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<A> repeatUntil<A>(
        Schedule schedule,
        Eff<A> ma,
        Func<A, bool> predicate) =>
        ma.RepeatUntil(schedule, predicate);
}
