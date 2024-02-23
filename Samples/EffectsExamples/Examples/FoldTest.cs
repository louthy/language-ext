using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
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
    public class FoldTest<RT>  
        where RT : Has<Eff<RT>, ConsoleIO>
    {
        public static Eff<RT, Unit> main =>
            mainEffect.RunEffect().As();
        
        static Effect<Eff<RT>, Unit> mainEffect =>
            repeat(Console<Eff<RT>, RT>.readKeys)
                | exitIfEscape
                | keyChar
                | words
                | filterEmpty
                | writeLine;

        static Pipe<ConsoleKeyInfo, ConsoleKeyInfo, Eff<RT>, Unit> exitIfEscape =>
            from k in awaiting<ConsoleKeyInfo>()
            from x in guard(k.Key != ConsoleKey.Escape, Errors.Cancelled)
            from _ in yield(k)
            select unit;
        
        static Pipe<ConsoleKeyInfo, char, Eff<RT>, Unit> keyChar =>
            map<ConsoleKeyInfo, char>(k => k.KeyChar);
    
        static Pipe<char, string, Eff<RT>, Unit> words =>
            foldUntil("", (word, ch) => word + ch, (char x) => char.IsWhiteSpace(x));

        static Pipe<string, string, Eff<RT>, Unit> filterEmpty =>
            filter<string>(notEmpty);
        
        static Consumer<string, Eff<RT>, Unit> writeLine =>
            from l in awaiting<string>()
            from _ in Console<Eff<RT>, RT>.writeLine(l)
            select unit;
    }
}
