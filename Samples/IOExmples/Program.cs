using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace IOExamples;

class Program
{
    static void Main(string[] args)
    {
        folding.Run(new MinRT()).IfLeft(Console.WriteLine);
    }

    static IO<MinRT, Error, Unit> infiniteLoop(int value) =>
        from _ in value % 100000 == 0
                    ? writeLine($"{value}")
                    : Pure(unit)
        from r in tail(infiniteLoop(value + 1))
        select unit;
    
    static IO<MinRT, Error, int> recursiveAskForNumber =>
        from n in askForNumber(1)
        from _ in writeLine($"Your number is: {n}")
        select n;
    
    static IO<MinRT, Error, int> askForNumber(int attempts) =>
        from _ in writeLine($"Enter a number (attempt number: {attempts})")
        from l in readLine
        from n in tail(int.TryParse(l, out var v)
                            ? Pure(v)
                            : from _ in writeLine("That's not a number!")
                              from r in tail(askForNumber(attempts + 1))
                              select r)
        select n;

    static readonly Transducer<int, int> folder =
        foldUntil(Schedule.spaced(TimeSpan.FromMilliseconds(100)),
                  0,
                  (int s, int x) => s + x,
                  valueIs: x => x % 10 == 0);
    
    static IO<MinRT, Error, Unit> folding =>
        from n in many(Range(1, 25))
        from _ in writeLine($"item flowing in: {n}")
        from s in n | folder
        from r in writeLine($"Total {s}")
        select unit;
    
    static IO<MinRT, Error, Unit> retrying =>
        from ix in many(Range(1, 10))
        from _1 in retry(from _2 in writeLine($"Enter a number to add to {ix}")
                         from nm in readLine.Map(parseInt)
                         from _3 in guard(nm.IsSome, Error.New("Please enter a valid integer"))
                         from _4 in writeLine($"Number {ix} + {(int)nm} = {ix + (int)nm}")
                         select unit)
        from _4 in waitOneSecond
        select unit;

    static IO<MinRT, Error, Unit> repeating =>
        repeat(
            from x in readNumber($"Enter the first number to add")
            from y in readNumber($"Enter the second number to add")
            from _ in writeLine($"{x} + {y} = {x + y}")
            from t in waitOneSecond
            select unit);
    
    static IO<MinRT, Error, int> readNumber(string question) =>
        (from _1 in writeLine(question)
         from nx in readLine.Map(parseInt)
         select nx)
        .RepeatWhile(mx => mx.IsNone)
        .Map(mx => (int)mx);
    
    static readonly IO<MinRT, Error, string> readLine =
        lift(() => Console.ReadLine() ?? "");
    
    static IO<MinRT, Error, Unit> writeLine(string line) =>
        lift(() =>
        {
            Console.WriteLine(line);
            return unit;
        });
    
    static IO<MinRT, Error, Unit> waitOneSecond =>
        liftIO(async _ => {
            await Task.Delay(1000);
            return unit;
        });
    
    static Transducer<Unit, DateTime> now =>
        lift(() => DateTime.Now);
}
