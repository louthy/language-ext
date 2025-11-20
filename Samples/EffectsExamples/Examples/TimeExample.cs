using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.UnitsOfMeasure;

namespace EffectsExamples;

/// <summary>
/// Clock example
/// </summary>
/// <remarks>
/// Prints the time for 10 repetitions, the space between the prints follows the Fibonacci sequence up to 10 seconds
/// and then it's clamped
/// </remarks>
public static class TimeExample<RT>
    where RT : 
        Has<Eff<RT>, TimeIO>, 
        Has<Eff<RT>, ConsoleIO>
{
    public static Eff<RT, Unit> main =>
       +repeat(Schedule.spaced(10 * second) | Schedule.fibonacci(1 * second) | Schedule.recurs(9),
               from tm in Time<RT>.now
               from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
               select unit);
}
