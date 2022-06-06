using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Error handling and guards example
    /// </summary>
    /// <remarks>
    /// Repeats the text you type in until you press Enter on an empty line, which will write a UserExited error - this
    /// will be caught for a safe exit
    /// Or, 'sys' that will throw a SystemException - this will be caught and 'sys error' will be printed
    /// Or, 'err' that will throw an Exception - this will be caught to become 'there was a problem'
    /// </remarks>
    public class ErrorAndGuardExample<RT>
        where RT : struct, HasConsole<RT>
    {
        static readonly Error UserExited = Error.New(100, "user exited");
        static readonly Error SafeError = Error.New(200, "there was a problem");

        public static Eff<RT, Unit> main =>
            from _1 in askUser
                     | @catch(ex => ex is SystemException, Console<RT>.writeLine("system error"))
                     | @catch(SafeError)
            from _2 in Console<RT>.writeLine("goodbye")
            select unit;

        static Eff<RT, Unit> askUser =>
            repeat(Schedule.spaced(1*second) | Schedule.recurs(3),
                   from ln in Console<RT>.readLine
                   from _1 in guard(notEmpty(ln), UserExited)
                   from _2 in guardnot(ln == "sys", () => throw new SystemException())
                   from _3 in guardnot(ln == "err", () => throw new Exception())
                   from _4 in Console<RT>.writeLine(ln)
                   select unit)
          | @catch(UserExited, unit);
    }
}
