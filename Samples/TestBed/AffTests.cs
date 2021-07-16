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
        
        public static Eff<RT, Unit> main =>
            from _1 in askUser
                          | match(UserExited, Console<RT>.writeLine("user existed"))
                          | match(ex => ex is SystemException, Console<RT>.writeLine("system error"))
            from _2 in Console<RT>.writeLine("goodbye")
            select unit;

        static Eff<RT, Unit> askUser =>
            repeat(from l in Console<RT>.readLine
                   from d in guard(notEmpty(l), UserExited)
                   from s in guard(l != "fuck", () => throw new SystemException())
                   from _ in Console<RT>.writeLine(l)
                   select unit);
    }
}
