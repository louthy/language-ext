using System.Text;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Pipes;
using LanguageExt.Sys.IO;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace EffectsExamples;

/// <summary>
/// Text file chunk streaming example
/// </summary>
/// <remarks>
/// Streams the contents of a text file in chunks of 40 characters
/// </remarks>
public static class TextFileChunkStreamExample<RT> 
    where RT : 
        Has<Eff<RT>, ConsoleIO>,
        Has<Eff<RT>, FileIO>,
        Has<Eff<RT>, TextReadIO>,
        Has<Eff<RT>, EncodingIO>
{
    public static Eff<RT, Unit> main =>
        from _ in Console<RT>.writeLine("Please type in a path to a text file and press enter")
        from p in Console<RT>.readLine
        from e in mainEffect(p)
        select unit;
        
    static Effect<Eff<RT>, Unit> mainEffect(string path) =>
        File<RT>.openRead(path) 
          | Stream<Eff<RT>>.read(80) 
          | decodeUtf8 
          | writeLine;

    static Pipe<SeqLoan<byte>, string, Eff<RT>, Unit> decodeUtf8 =>
        from c in awaiting<SeqLoan<byte>>()         
        from _ in yield(Encoding.UTF8.GetString(c.ToReadOnlySpan()))
        select unit;
        
    static Consumer<string, Eff<RT>, Unit> writeLine =>
        from l in awaiting<string>()
        from _ in Console<RT>.writeLine(l)
        select unit;
}
