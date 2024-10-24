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
/// Text file line streaming example
/// </summary>
/// <remarks>
/// Streams the contents of a text file, one line at a time
/// </remarks>
public class TextFileLineStreamExample<RT> 
    where RT : 
        Has<Eff<RT>, ConsoleIO>,
        Has<Eff<RT>, FileIO>,
        Has<Eff<RT>, TextReadIO>,
        Has<Eff<RT>, EncodingIO>
{
    public static Eff<RT, Unit> main =>
        from _ in Console<RT>.writeLine("Please type in a path to a text file and press enter")
        from p in Console<RT>.readLine
        from e in mainEffect(p).RunEffect()
        select unit;
        
    static Effect<Eff<RT>, Unit> mainEffect(string path) =>
        File<RT>.openText(path) 
            | TextRead<RT>.readLine 
            | writeLine;

    static Consumer<string, Eff<RT>, Unit> writeLine =>
        from l in awaiting<string>()
        from _ in Console<RT>.writeLine(l)
        select unit;
}
