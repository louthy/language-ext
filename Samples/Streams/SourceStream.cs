using LanguageExt;
using LanguageExt.Common;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public class SourceStream
{
    public static IO<Unit> run =>
        from s in Source<string>.Start()
        from f in show(s).Iter().ForkIO()
        from w in writeLine("Type something and press enter (empty-line ends the demo)")
        from r in interaction(s)
        select r;

    static IO<Unit> interaction(Source<string> source) =>
        from l in readLine
        from _ in l == ""
                      ? Pure(unit)
                      : source.Post(l)
                              .Bind(_ => interaction(source))
        select unit;

    static StreamT<IO, Unit> show(Source<string> source) =>
        from v in source.Await<IO>()
        from _ in writeLine(v)
        where false
        select unit;
}
