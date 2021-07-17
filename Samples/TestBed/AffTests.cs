using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class AffTests<RT>
        where RT : struct, HasConsole<RT>
    {
        static readonly Error UserExited = Error.New(100, "user exited");
        static readonly Error SafeError = Error.New(200, "there was a problem");
        
        public static Eff<RT, Unit> main =>
            from _1 in askUser
                          | @catch(UserExited, Console<RT>.writeLine("we're sad to see you go"))
                          | @catch(ex => ex is SystemException, from _ in Console<RT>.writeLine("system error")
                                                                from e in FailEff<Unit>(SafeError)
                                                                select e)
                          | @catch(SafeError)
            from _2 in Console<RT>.writeLine("goodbye")
            select unit;

        static Eff<RT, Unit> askUser =>
            repeat(from l in Console<RT>.readLine
                   from d in guard(notEmpty(l), UserExited)
                   from s in notguard(l == "err", () => throw new SystemException())
                   from _ in Console<RT>.writeLine(l)
                   select unit);
    }
}
