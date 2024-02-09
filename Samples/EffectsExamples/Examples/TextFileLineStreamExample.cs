using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Sys.IO;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples
{
    /// <summary>
    /// Text file line streaming example
    /// </summary>
    /// <remarks>
    /// Streams the contents of a text file, one line at a time
    /// </remarks>
    public class TextFileLineStreamExample<RT> where RT : 
        struct, 
        HasCancel<RT>,
        HasConsole<RT>,
        HasFile<RT>,
        HasTextRead<RT>
    {
        public static Aff<RT, Unit> main =>
            from _ in Console<RT>.writeLine("Please type in a path to a text file and press enter")
            from p in Console<RT>.readLine
            from e in mainEffect(p)
            select unit;
        
        static Effect<RT, Unit> mainEffect(string path) =>
            File<RT>.openText(path) 
              | TextRead<RT>.readLine 
              | writeLine;

        static Consumer<RT, string, Unit> writeLine =>
            from l in awaiting<string>()
            from _ in Console<RT>.writeLine(l)
            select unit;
    }
}
