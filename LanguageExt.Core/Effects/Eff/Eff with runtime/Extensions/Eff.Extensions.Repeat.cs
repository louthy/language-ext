using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Keeps repeating the computation   
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> Repeat<RT, A>(this K<Eff<RT>, A> ma) =>
        ma.As().MapIO(io => io.Repeat());

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> Repeat<RT, A>(this K<Eff<RT>, A> ma, Schedule schedule) =>
        ma.As().MapIO(io => io.Repeat(schedule));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> RepeatWhile<RT, A>(this K<Eff<RT>, A> ma, Func<A, bool> predicate) =>
        ma.As().MapIO(io => io.RepeatWhile(predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> RepeatWhile<RT, A>(
        this K<Eff<RT>, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma.As().MapIO(io => io.RepeatWhile(schedule, predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> RepeatUntil<RT, A>(
        this K<Eff<RT>, A> ma,
        Func<A, bool> predicate) =>
        ma.As().MapIO(io => io.RepeatUntil(predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> RepeatUntil<RT, A>(
        this K<Eff<RT>, A> ma,
        Schedule schedule,
        Func<A, bool> predicate) =>
        ma.As().MapIO(io => io.RepeatUntil(schedule, predicate));
}
