using System;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Producer;
using static LanguageExt.Pipes.Pipe;
using static LanguageExt.Pipes.Consumer;

namespace EffectsExamples
{
    /// <summary>
    /// Fold test
    /// </summary>
    /// <remarks>
    /// Processes keys from the keyboard into words, when a whitespace is encountered the folded word is yielded
    /// down the pipe 
    /// </remarks>
    public static class FoldTest<RT>  
        where RT : 
            Has<Eff<RT>, ConsoleIO>
    {
        public static Eff<RT, Unit> main =>
            mainEffect.Run().As();
        
        static Effect<RT, Unit> mainEffect =>
            repeat(Console<RT>.readKeys)
                | exitIfEscape
                | keyChar
                | words
                | filterEmpty
                | writeLine;

        static Pipe<RT, ConsoleKeyInfo, ConsoleKeyInfo, Unit> exitIfEscape =>
           (from k in awaiting<RT, ConsoleKeyInfo, ConsoleKeyInfo>()
            from x in guard(k.Key != ConsoleKey.Escape, Errors.Cancelled)
            from _ in yield<RT, ConsoleKeyInfo, ConsoleKeyInfo>(k)
            select unit)
          .As();
        
        static Pipe<RT, ConsoleKeyInfo, char, Unit> keyChar =>
            map<RT, ConsoleKeyInfo, char>(k => k.KeyChar);
    
        static Pipe<RT, char, string, Unit> words =>
            foldUntil<RT, char, string>((word, ch) => word + ch, x => char.IsWhiteSpace(x.Value), "");

        static Pipe<RT, string, string, Unit> filterEmpty =>
            filter<RT, string>(notEmpty);
        
        static Consumer<RT, string, Unit> writeLine =>
            from l in awaiting<RT, string>()
            from _ in Console<RT>.writeLine(l)
            select unit;
    }
}
