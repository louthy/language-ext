using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.UnitsOfMeasure;

namespace EffectsExamples;

/// <summary>
/// Process timeout example
/// </summary>
/// <remarks>
/// Repeats a backing off process for 1 minutes
/// The back-off follows the fibonacci sequence in terms of the delay
/// </remarks>
/// <typeparam name="RT"></typeparam>
public class TimeoutExample<RT>
    where RT : 
    Has<Eff<RT>, TimeIO>, 
    Has<Eff<RT>, ConsoleIO>
{
    public static Eff<RT, Unit> main =>
        from _1 in timeout(60 * seconds, longRunning)
                 | @catch(Errors.TimedOut, pure<Eff<RT>, Unit>(unit))
        from _2 in Console<Eff<RT>, RT>.writeLine("done")
        select unit;

    static Eff<RT, Unit> longRunning =>
        (from tm in Time<Eff<RT>, RT>.now
         from _1 in Console<Eff<RT>, RT>.writeLine(tm.ToLongTimeString())
         select unit)
       .Repeat(Schedule.fibonacci(1 * second));
}
