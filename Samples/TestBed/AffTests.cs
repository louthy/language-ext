using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;

namespace TestBed
{
    public class AffTests
    {
        public static Eff<RT, Unit> Main<RT>()
            where RT : struct, HasConsole<RT> =>
            AskUser<RT>() 
          | Catch(666, unit)
          | CatchEx(ex => ex is SystemException, Console<RT>.writeLine("System error"));

        static Eff<RT, Unit> AskUser<RT>()
            where RT : struct, HasConsole<RT> =>
                repeat(from l in Console<RT>.readLine
                       from d in guard(l != "", Error.New(666, "user exited"))
                       from _ in Console<RT>.writeLine($"{l}")
                       select unit);
    }
}
