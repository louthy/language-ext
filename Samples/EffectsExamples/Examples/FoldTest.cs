using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples
{
    /// <summary>
    /// Fold test
    /// </summary>
    /// <remarks>
    /// Processes keys from the keyboard into words, when a whitespace is encountered the folded word is yielded
    /// down the pipe 
    /// </remarks>
    public class FoldTest<RT> where RT : 
        struct, 
        HasCancel<RT>,
        HasConsole<RT>
    {
        public static Aff<RT, Unit> main =>
            mainEffect.RunEffect();
        
        static Effect<RT, Unit> mainEffect =>
            repeat(Console<RT>.readKeys)
                | exitIfEscape
                | keyChar
                | words
                | filterEmpty
                | writeLine;

        static Pipe<RT, ConsoleKeyInfo, ConsoleKeyInfo, Unit> exitIfEscape =>
            from k in awaiting<ConsoleKeyInfo>()
            from x in guardnot(k.Key == ConsoleKey.Escape, Errors.Cancelled).ToAff<RT>()
            from _ in yield(k)
            select unit;
        
        static Pipe<RT, ConsoleKeyInfo, char, Unit> keyChar =>
            map<ConsoleKeyInfo, char>(k => k.KeyChar);
    
        static Pipe<RT, char, string, Unit> words =>
            foldUntil("", (word, ch) => word + ch, (char x) => char.IsWhiteSpace(x));

        static Pipe<RT, string, string, Unit> filterEmpty =>
            filter<string>(notEmpty);
        
        static Consumer<RT, string, Unit> writeLine =>
            from l in awaiting<string>()
            from _ in Console<RT>.writeLine(l)
            select unit;
    }
}
