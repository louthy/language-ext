using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Clock example
    /// </summary>
    /// <remarks>
    /// Prints the time for 10 repetitions, the space between the prints follows the Fibonacci sequence up to 10 seconds
    /// and then it's clamped
    /// </remarks>
    public class TimeExample<RT>
        where RT : 
        HasTime<RT>, 
        HasCancel<RT>, 
        HasConsole<RT>
    {
        public static Eff<RT, Unit> main =>
            repeat(Schedule.spaced(10 * second) | Schedule.fibonacci(1 * second) | Schedule.recurs(9),
                   from tm in Time<RT>.now
                   from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
                   select unit);
    }
}
