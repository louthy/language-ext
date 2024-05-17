using System;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace IOExamples;

class Program
{
    static void Main(string[] args)
    {
        infiniteLoop(0).Run();
    }

    static IO<Unit> infiniteLoop(int value) =>
        from _ in value % 1 == 0
                    ? writeLine($"{value}")
                    : Pure(unit)
        from x in wait(1)
        from r in infiniteLoop(value + 1)
        select unit;
    
    static IO<int> recursiveAskForNumber =>
        from n in askForNumber(1)
        from _ in writeLine($"Your number is: {n}")
        select n;

    static IO<int> askForNumber(int attempts) =>
        from _ in writeLine($"Enter a number (attempt number: {attempts})")
        from l in readLine
        from n in int.TryParse(l, out var v)
                      ? Pure(v)
                      : from _ in writeLine("That's not a number!")
                        from r in askForNumber(attempts + 1)
                        select r
        select n;

    // static readonly Transducer<int, int> folder =
    //     foldUntil(Schedule.spaced(TimeSpan.FromMilliseconds(100)),
    //               0,
    //               (int s, int x) => s + x,
    //               valueIs: x => x % 10 == 0);
    //
    // static IO<Unit> folding =>
    //     from n in many(Range(1, 25))
    //     from _ in writeLine($"item flowing in: {n}")
    //     from s in n | folder
    //     from r in writeLine($"Total {s}")
    //     select unit;
    
    // static IO<Unit> retrying =>
    //     from ix in many(Range(1, 10))
    //     from _1 in retry(from _2 in writeLine($"Enter a number to add to {ix}")
    //                      from nm in readLine.Map(parseInt)
    //                      from _3 in guard(nm.IsSome, Error.New("Please enter a valid integer"))
    //                      from _4 in writeLine($"Number {ix} + {(int)nm} = {ix + (int)nm}")
    //                      select unit)
    //     from _4 in waitOneSecond
    //     select unit;

    static IO<Unit> repeating =>
        repeat(Schedule.recurs(5),
               from x in readNumber("Enter the first number to add")
               from y in readNumber("Enter the second number to add")
               from _ in writeLine($"{x} + {y} = {x + y}")
               from t in waitOneSecond
               select unit)
      | catchM(writeLine("Obviously you don't know what a number is so I'm off."));
    
    static IO<int> readNumber(string question) =>
        retry(Schedule.recurs(3),
              from _1 in writeLine(question)
              from nx in readLine.Map(int.Parse)
              select nx);
    
    static readonly IO<string> readLine =
        lift(() => Console.ReadLine() ?? "");
    
    static IO<Unit> writeLine(string line) =>
        lift(() =>
        {
            Console.WriteLine(line);
            return unit;
        });
    
    static IO<Unit> waitOneSecond =>
        wait(1000);

    static IO<Unit> wait(int milliseconds) =>
        IO.yield(milliseconds);

    static IO<DateTime> now =>
        lift(() => DateTime.Now);
}
