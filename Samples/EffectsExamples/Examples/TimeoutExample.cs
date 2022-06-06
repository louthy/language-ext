using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Process timeout example
    /// </summary>
    /// <remarks>
    /// Repeats a backing off process for 1 minutes
    /// The back-off follows the fibonacci sequence in terms of the delay
    /// </remarks>
    /// <typeparam name="RT"></typeparam>
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
           .Repeat(Schedule.fibonacci(1 * second));
    }
}
