#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

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
    public static Eff<RT, A> repeat<RT, A>(Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        new(repeat(ma.As));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires  
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> repeat<RT, A>(Schedule schedule, Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        new(repeat(schedule, ma.As));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns false
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> repeatWhile<RT, A>(
        Eff<RT, A> ma,
        Func<A, bool> predicate) where RT : struct, HasIO<RT, Error> =>
        new(repeatWhile(ma.As, predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns false
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> repeatWhile<RT, A>(
        Schedule schedule,
        Eff<RT, A> ma,
        Func<A, bool> predicate)
        where RT : struct, HasIO<RT, Error> =>
        new(repeatWhile(schedule, ma.As, predicate));

    /// <summary>
    /// Keeps repeating the computation until the predicate returns true
    /// </summary>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> repeatUntil<RT, A>(
        Eff<RT, A> ma,
        Func<A, bool> predicate)
        where RT : struct, HasIO<RT, Error> =>
        new(repeatUntil(ma.As, predicate));

    /// <summary>
    /// Keeps repeating the computation, until the scheduler expires, or the predicate returns true
    /// </summary>
    /// <param name="schedule">Scheduler strategy for repeating</param>
    /// <param name="ma">Computation to repeat</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Computation bound value type</typeparam>
    /// <returns>The result of the last invocation of `ma`</returns>
    public static Eff<RT, A> repeatUntil<RT, A>(
        Schedule schedule,
        Eff<RT, A> ma,
        Func<A, bool> predicate)
        where RT : struct, HasIO<RT, Error> =>
        new(repeatUntil(schedule, ma.As, predicate));
}
