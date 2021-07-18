using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;


namespace TestBed
{
    public class EffectsTest1<RT>
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
            repeat(Schedule.Spaced(1*second) | Schedule.Recurs(3),
                   from _0 in Console<RT>.writeLine("Welcome")
                   from ln in Console<RT>.readLine
                   from _1 in guard(notEmpty(ln), UserExited)
                   from _2 in notguard(ln == "sys", () => throw new SystemException())
                   from _3 in notguard(ln == "err", () => throw new Exception())
                   from _4 in Console<RT>.writeLine(ln)
                   select unit)
          | @catch(UserExited, unit);
    }

    public class EffectsTest2<RT>
        where RT : 
            struct, 
            HasTime<RT>, 
            HasCancel<RT>, 
            HasConsole<RT>
    {
        public static Aff<RT, Unit> main =>
            repeat(Schedule.Spaced(1 * second),
                   from tm in DateTime<RT>.now
                   from _1 in Console<RT>.writeLine(tm.ToLongTimeString())
                   select unit);
    }
}
