using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    public class TimeExample<RT>
        where RT : struct, 
        HasTime<RT>, 
        HasCancel<RT>, 
        HasConsole<RT>
    {
        public static Eff<RT, Unit> main =>
            repeat(Schedule.Spaced(10 * second) | Schedule.Recurs(15) | Schedule.Fibonacci(1*second),
                   from tm in Time<RT>.now
                   from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
                   select unit);
    }
}
