using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
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
