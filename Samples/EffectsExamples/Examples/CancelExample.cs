using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Cancel example 
    /// </summary>
    /// <remarks>
    /// Accepts key presses and echos them to the console until Enter is pressed.
    /// When Enter is pressed it calls `cancel<RT>()` to trigger the cancellation token
    /// </remarks>
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
