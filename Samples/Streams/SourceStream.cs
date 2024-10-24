using LanguageExt;
using LanguageExt.Common;
using static Streams.Console;
using static LanguageExt.Prelude;

namespace Streams;

public class SourceStream
{
    public static IO<Unit> run =>
        from s in Source<string>()
        from f in fork(subscribe(s))
        from _ in writeLine("Type something and press enter (empty-line ends the demo)") >>
                  interaction(s)
        select unit;

    static IO<Unit> interaction(Source<string> source) =>
        repeat(from l in readLine
               from _ in deliver(source, l)
               select unit) 
      | @catch(unitIO);

    static IO<Unit> deliver(Source<string> source, string line) =>
        guardIO(line != "") >>
        post(source, line);

    static StreamT<IO, Unit> subscribe(Source<string> source) =>
        from v in await<IO, string>(source)
        from _ in writeLine(v)
        where false
        select unit;
}
