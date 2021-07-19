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
    public class TimeExample<RT>
        where RT : struct, 
        HasTime<RT>, 
        HasCancel<RT>, 
        HasConsole<RT>
    {
        public static Eff<RT, Unit> main =>
            repeat(Schedule.Spaced(1 * second) | Schedule.Recurs(15),
                   from tm in Time<RT>.now
                   from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
                   select unit);
    }
}
