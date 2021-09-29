using System.Text;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Sys.IO;
using LanguageExt.Common;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples
{
    /// <summary>
    /// Text file chunk streaming example
    /// </summary>
    /// <remarks>
    /// Streams the contents of a text file in chunks of 40 characters
    /// </remarks>
    public class TextFileChunkStreamExample<RT> where RT : 
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
            File<RT>.openRead(path) 
               | Stream<RT>.read(80) 
               | decodeUtf8 
               | writeLine;

        static Pipe<RT, SeqLoan<byte>, string, Unit> decodeUtf8 =>
            from c in awaiting<SeqLoan<byte>>()         
            from _ in yield(Encoding.UTF8.GetString(c.ToReadOnlySpan()))
            select unit;
        
        static Consumer<RT, string, Unit> writeLine =>
            from l in awaiting<string>()
            from _ in Console<RT>.writeLine(l)
            select unit;
    }
}
