using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    public class TimeoutExample<RT>
        where RT : struct,
        HasTime<RT>,
        HasCancel<RT>,
        HasConsole<RT>
    {
        public static Aff<RT, Unit> main =>
            from _1 in timeout(60 * seconds, longRunning)
                     | @catch(Errors.TimedOut, unit)
            from _2 in Console<RT>.writeLine("done")
            select unit;

        static Aff<RT, Unit> longRunning =>
            (from tm in Time<RT>.now
             from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
             select unit)
           .ToAff()
           .Repeat(Schedule.Fibonacci(1 * second));
    }
}
