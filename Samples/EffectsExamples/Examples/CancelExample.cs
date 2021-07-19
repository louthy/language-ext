using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    public class CancelExample<RT>
        where RT: struct, 
        HasCancel<RT>, 
        HasConsole<RT>
    {
        public static Aff<RT, Unit> main =>
            repeat(from k in Console<RT>.readKey
                   from _ in k.Key == ConsoleKey.Enter
                                 ? cancel<RT>()
                                 : unitEff
                   from w in Console<RT>.write(k.KeyChar)
                   select unit);
    }
}
