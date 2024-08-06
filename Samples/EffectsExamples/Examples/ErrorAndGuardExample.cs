using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.UnitsOfMeasure;

namespace EffectsExamples;

/// <summary>
/// Error handling and guards example
/// </summary>
/// <remarks>
/// Repeats the text you type in until you press Enter on an empty line, which will write a UserExited error - this
/// will be caught for a safe exit
/// Or, 'sys' that will throw a SystemException - this will be caught and 'sys error' will be printed
/// Or, 'err' that will throw an Exception - this will be caught to become 'there was a problem'
/// </remarks>
public static class ErrorAndGuardExample<RT>
    where RT : 
        Has<Eff<RT>, ConsoleIO>
{
    static readonly Error UserExited = Error.New(100, "user exited");
    static readonly Error SafeError = Error.New(200, "there was a problem");

    public static Eff<RT, Unit> main =>
        from _1 in askUser
                 | @catch(ex => ex.Is<SystemException>(), Console<Eff<RT>, RT>.writeLine("system error"))
                 | SafeError
        from _2 in Console<Eff<RT>, RT>.writeLine("goodbye")
        select unit;

    static Eff<RT, Unit> askUser =>
        repeatIO(Schedule.spaced(1 * second) | Schedule.recurs(3),
                 from ln in Console<Eff<RT>, RT>.readLine
                 from _1 in guard(notEmpty(ln), UserExited)
                 from _2 in guard(ln != "sys", () => throw new SystemException())
                 from _3 in guard(ln != "err", () => throw new Exception())
                 from _4 in Console<Eff<RT>, RT>.writeLine(ln)
                 select unit).As()
      | @catch(UserExited, pure<Eff<RT>, Unit>(unit));
}
