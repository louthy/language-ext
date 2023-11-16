using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace IOExamples;

class Program
{
    static void Main(string[] args)
    {
        var r = infiniteLoop(1);
            
        Console.WriteLine(r.Run(new MinimalRT()));
    }

    static IO<MinimalRT, Error, Unit> infiniteLoop(int value) =>
        from _ in writeLine(value.ToString())
        from r in infiniteLoop(value + 1)
        select unit;
    
    static IO<MinimalRT, Error, int> recursiveAskForNumber =>
        from x in Pure(100)
        from y in Pure(200)
        from z in askForNumber(1)
        select x + y + z;
    
    static IO<MinimalRT, Error, int> askForNumber(int attempts) =>
        from _ in writeLine($"Enter a number (attempt number: {attempts})")
        from l in readLine
        from n in int.TryParse(l, out var v)
            ? Pure(v)
            : from _ in writeLine("That's not a number!")
              from r in askForNumber(attempts + 1)
              select r
        select n;

    static readonly IO<MinimalRT, Error, string> readLine =
        liftIO<MinimalRT, Error, string>(_ => Console.ReadLine() ?? "");
    
    static IO<MinimalRT, Error, Unit> writeLine(string line) =>
        liftIO<MinimalRT, Error, Unit>(_ =>
        {
            Console.WriteLine(line);
            return unit;
        });
}
