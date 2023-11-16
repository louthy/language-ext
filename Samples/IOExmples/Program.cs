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
        retrying.Run(new MinimalRT());
    }

    static IO<MinimalRT, Error, Unit> infiniteLoop(int value) =>
        from _ in value % 100000 == 0
                    ? writeLine($"{value}")
                    : Pure(unit)
        from r in tail(infiniteLoop(value + 1))
        select unit;
    
    static IO<MinimalRT, Error, int> recursiveAskForNumber =>
        from n in askForNumber(1)
        from _ in writeLine($"Your number is: {n}")
        select n;
    
    static IO<MinimalRT, Error, int> askForNumber(int attempts) =>
        from _ in writeLine($"Enter a number (attempt number: {attempts})")
        from l in readLine
        from n in tail(int.TryParse(l, out var v)
                            ? Pure(v)
                            : from _ in writeLine("That's not a number!")
                              from r in tail(askForNumber(attempts + 1))
                              select r)
        select n;

    static IO<MinimalRT, Error, Unit> retrying =>
        from ix in many(Range(1, 10))
        from _1 in retry(
            from _2 in writeLine($"Enter a number to add to {ix}")
            from nm in readLine.Map(int.Parse)
            from _3 in writeLine($"Number {ix} + {nm} = {ix + nm}")
            select unit)
        from _4 in waitOneSecond
        select unit;
    
    static readonly IO<MinimalRT, Error, string> readLine =
        liftIO<MinimalRT, Error, string>(_ => Console.ReadLine() ?? "");
    
    static IO<MinimalRT, Error, Unit> writeLine(string line) =>
        liftIO<MinimalRT, Error, Unit>(_ =>
        {
            Console.WriteLine(line);
            return unit;
        });
    
    static IO<MinimalRT, Error, Unit> waitOneSecond =>
        liftIO<MinimalRT, Error, Unit>(async _ => {
            await Task.Delay(1000);
            return unit;
        });
    
}
